using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq; // <-- Importante para JSON
using System;
using System.Collections.Generic; // <-- Importante para Listas y Diccionarios
using System.Linq; // <-- Importante para .Select y .Sum
using System.Text; // <-- Importante para StringBuilder
using System.Windows.Forms;
using XrmToolBox.Extensibility;

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
            // Nada que hacer al cerrar por ahora
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            LogInfo("Conexión actualizada.");

            // Limpiamos los controles si la conexión cambia
            lvSolutions.Items.Clear();
            rtbResults.Clear();
            txtSearchTerm.Clear();
        }

        // --- PASO 1: CARGAR SOLUCIONES ---

        private void btnFetchSolutions_Click(object sender, EventArgs e)
        {
            // Limpiamos los controles
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

        // --- PASO 2: BUSCAR TEXTO EN FLUJOS ---

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

            // Usamos una lambda para pasar parámetros a ExecuteMethod
            ExecuteMethod(() => SearchInFlows(selectedSolutionId, searchTerm));
        }

        private void SearchInFlows(Guid solutionId, string searchTerm)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Analizando flujos en la solución...",
                Work = (worker, args) =>
                {
                    // 1. Consulta
                    var query = new QueryExpression("workflow");
                    query.ColumnSet.AddColumns("name", "clientdata"); // Pedimos el JSON
                    query.Criteria.AddCondition("category", ConditionOperator.Equal, 5); // Power Automate

                    // 2. Vínculo (Join)
                    var link = new LinkEntity("workflow", "solutioncomponent", "workflowid", "objectid", JoinOperator.Inner);
                    link.LinkCriteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId);
                    link.LinkCriteria.AddCondition("componenttype", ConditionOperator.Equal, 29); // Workflow
                    query.LinkEntities.Add(link);

                    // 3. Ejecutar
                    EntityCollection retrievedFlows = Service.RetrieveMultiple(query);

                    // 4. Analizar JSON
                    // Usamos un Diccionario para agrupar resultados por nombre de flujo
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

                    // 5. Devolver Diccionario
                    args.Result = foundResults;
                },
                PostWorkCallBack = (args) =>
                {
                    // 6. Mostrar Resultados Agrupados
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
                        sb.AppendLine($"Flujo: {entry.Key}"); // Nombre del flujo
                        foreach (var path in entry.Value)
                        {
                            sb.AppendLine($"    Ruta: {path}"); // Rutas con sangría
                        }
                        sb.AppendLine(); // Espacio entre flujos
                    }

                    rtbResults.AppendText(sb.ToString());
                }
            });
        }

        /// <summary>
        /// Analiza recursivamente las acciones de un flujo buscando un término.
        /// </summary>
        private void FindInActions(JToken token, string searchTerm, string fileName, string currentPath, Dictionary<string, List<string>> results)
        {
            foreach (var prop in token.Children<JProperty>())
            {
                string actionName = prop.Name;
                string newPath = $"{currentPath} -> {actionName}";
                JToken actionContent = prop.Value;

                // --- INICIO MEJORA: Búsqueda Limpia ---
                JObject contentToSearch = actionContent.DeepClone() as JObject;
                if (contentToSearch != null)
                {
                    // Eliminamos cajas hijas para no buscar dentro de ellas
                    contentToSearch.Remove("actions");
                    contentToSearch.Remove("iftrue");
                    contentToSearch.Remove("iffalse");
                    contentToSearch.Remove("else");
                    contentToSearch.Remove("cases");
                }

                string actionText = (contentToSearch ?? actionContent).ToString();

                // Buscamos solo en el texto de esta caja
                if (actionText.Contains(searchTerm))
                {
                    // ¡Encontrado! Añadir al diccionario
                    if (!results.ContainsKey(fileName))
                    {
                        results.Add(fileName, new List<string>());
                    }
                    results[fileName].Add(newPath);
                }
                // --- FIN MEJORA ---

                // --- Búsqueda Recursiva (en el contenido original) ---
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