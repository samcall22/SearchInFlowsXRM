using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq; //JSON
using System;
using System.Collections.Generic; // List y Dict
using System.Linq; //.Select y .Sum
using System.Text; //StringBuilder
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using static System.Net.Mime.MediaTypeNames;

namespace SearchInFlows
{
    public partial class MyPluginControl : PluginControlBase
    {
        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            LogInfo("Plugin cargado.");
        }

        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Nothing to do when closing for now
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            LogInfo("Conexión actualizada.");

            // We clean the controls if the connection changes
            lvSolutions.Items.Clear();
            rtbResults.Clear();
            txtSearchTerm.Clear();
        }

        //--- STEP 1: LOAD SOLUTIONS ---

        private void btnFetchSolutions_Click(object sender, EventArgs e)
        {
            // We clean the controls
            lvSolutions.Items.Clear();
            rtbResults.Clear();
            txtSearchTerm.Clear();

            ExecuteMethod(FetchSolutions);
        }

        private void FetchSolutions()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Consultando soluciones en Dataverse...",
                Work = (worker, args) =>
                {
                    var query = new QueryExpression("solution");
                    query.ColumnSet.AddColumns("friendlyname", "uniquename", "solutionid");
                    query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);
                    query.Criteria.AddCondition("uniquename", ConditionOperator.NotEqual, "Default");
                    query.Orders.Add(new OrderExpression("friendlyname", OrderType.Ascending));

                    args.Result = Service.RetrieveMultiple(query);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var retrievedSolutions = args.Result as EntityCollection;
                    if (retrievedSolutions.Entities.Count == 0)
                    {
                        MessageBox.Show("No se encontraron soluciones visibles.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    foreach (var entity in retrievedSolutions.Entities)
                    {
                        var row = new ListViewItem(entity.GetAttributeValue<string>("friendlyname"));
                        row.SubItems.Add(entity.GetAttributeValue<string>("uniquename"));
                        row.Tag = entity.Id; // Guardamos el ID en la fila
                        lvSolutions.Items.Add(row);
                    }
                }
            });
        }

        // --- STEP 2: SEARCH FOR TEXT IN STREAMS ---

        private void btnFetchFlows_Click(object sender, EventArgs e)
        {
            rtbResults.Clear();

            if (lvSolutions.SelectedItems.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona una solución de la lista primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSearchTerm.Text))
            {
                MessageBox.Show("Por favor, introduce un término de búsqueda.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ListViewItem selectedRow = lvSolutions.SelectedItems[0];
            Guid selectedSolutionId = (Guid)selectedRow.Tag;
            string searchTerm = txtSearchTerm.Text;

            // We use a lambda to pass parameters to ExecuteMethod
            ExecuteMethod(() => SearchInFlows(selectedSolutionId, searchTerm));
        }

        private void SearchInFlows(Guid solutionId, string searchTerm)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Analizando flujos en la solución...",
                Work = (worker, args) =>
                {
                    // 1. Consultation
                    var query = new QueryExpression("workflow");
                    query.ColumnSet.AddColumns("name", "clientdata"); // Pedimos el JSON
                    query.Criteria.AddCondition("category", ConditionOperator.Equal, 5); // Power Automate

                    // 2. Link (Join)
                    var link = new LinkEntity("workflow", "solutioncomponent", "workflowid", "objectid", JoinOperator.Inner);
                    link.LinkCriteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId);
                    link.LinkCriteria.AddCondition("componenttype", ConditionOperator.Equal, 29); // Workflow
                    query.LinkEntities.Add(link);

                    // 3.Run
                    EntityCollection retrievedFlows = Service.RetrieveMultiple(query);

                    // 4. Parse JSON
                    // We use a Dictionary to group results by stream name
                    var foundResults = new Dictionary<string, List<string>>();

                    foreach (var flowEntity in retrievedFlows.Entities)
                    {
                        string flowName = flowEntity.GetAttributeValue<string>("name");
                        string jsonContent = flowEntity.GetAttributeValue<string>("clientdata");

                        if (string.IsNullOrEmpty(jsonContent)) continue;

                        try
                        {
                            JObject parsedJson = JObject.Parse(jsonContent);
                            JToken actions = parsedJson.SelectToken("properties.definition.actions");

                            if (actions != null)
                            {
                                FindInActions(actions, searchTerm, flowName, "root", foundResults);
                            }
                        }
                        catch (Exception ex)
                        {
                            foundResults.Add($"Error al analizar '{flowName}'", new List<string> { ex.Message });
                        }
                    }

                    // 5. Return Dictionary
                    args.Result = foundResults;
                },
                PostWorkCallBack = (args) =>
                {
                    // 6. Show Grouped Results
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var results = args.Result as Dictionary<string, List<string>>;
                    if (results.Count == 0)
                    {
                        rtbResults.Text = $"No se encontró el término '{searchTerm}'.";
                        return;
                    }

                    var totalFound = results.Sum(kvp => kvp.Value.Count);
                    var sb = new StringBuilder();
                    sb.AppendLine($"Se encontraron {totalFound} coincidencias:\n");

                    foreach (var entry in results)
                    {
                        sb.AppendLine($"Flujo: {entry.Key}"); // FLow name
                        foreach (var path in entry.Value)
                        {
                            sb.AppendLine($"    Ruta: {path}"); // Routes with sangria
                        }
                        sb.AppendLine(); // Space between flows
                    }

                    rtbResults.AppendText(sb.ToString());
                }
            });
        }

        /// <summary>
        /// Recursively analyzes the actions in a stream looking for a term.
        /// </summary>
        private void FindInActions(JToken token, string searchTerm, string fileName, string currentPath, Dictionary<string, List<string>> results)
        {
            foreach (var prop in token.Children<JProperty>())
            {
                string actionName = prop.Name;
                string newPath = $"{currentPath} -> {actionName}";
                JToken actionContent = prop.Value;

                // --- START IMPROVEMENT: Clean Search ---
                JObject contentToSearch = actionContent.DeepClone() as JObject;
                if (contentToSearch != null)
                {
                    // We eliminated daughter boxes so as not to search inside them
                    contentToSearch.Remove("actions");
                    contentToSearch.Remove("iftrue");
                    contentToSearch.Remove("iffalse");
                    contentToSearch.Remove("else");
                    contentToSearch.Remove("cases");
                }

                string actionText = (contentToSearch ?? actionContent).ToString();

                // We search only in the text in this box
                if (actionText.Contains(searchTerm))
                {
                    // Found! Add to dictionary
                    if (!results.ContainsKey(fileName))
                    {
                        results.Add(fileName, new List<string>());
                    }
                    results[fileName].Add(newPath);
                }

                // --- END IMPROVEMENT ---

                // --- Recursive Search (in original content) ---
                var nestedActions = actionContent.SelectToken("actions");
                if (nestedActions != null)
                {
                    FindInActions(nestedActions, searchTerm, fileName, newPath, results);
                }

                var ifTrueActions = actionContent.SelectToken("iftrue.actions");
                if (ifTrueActions != null)
                {
                    FindInActions(ifTrueActions, searchTerm, fileName, newPath + " (Rama 'Sí')", results);
                }

                var ifFalseActions = actionContent.SelectToken("iffalse.actions");
                if (ifFalseActions != null)
                {
                    FindInActions(ifFalseActions, searchTerm, fileName, newPath + " (Rama 'No')", results);
                }
            }
        }
    }
}