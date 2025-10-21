# Search in Automate (Samcall.XTB.FlowSearch)

A plugin for XrmToolBox that allows you to search for a specific text string within all Power Automate flows belonging to a solution.

## Description

This plugin saves you hours of manual work. Instead of opening each flow one by one to find where a specific variable, API key, or text string is used, this tool does it for you, returning the path of actions (scopes, conditions, etc.) to the action where the string was found.

## Features

* Loads all visible solutions from your environment.
* Lists all Power Automate flows that are components of the selected solution.
* Analyzes the JSON of each flow to find a text string.
* Displays the results grouped by flow.
* Indicates the "action path" (e.g., `root -> Scope_My_Action -> Condition`) where the match was found.

## How to Use

1.  Open the tool and connect to your Dataverse environment.
2.  Click on **"Load Solutions"**.
3.  Select the solution you want to analyze from the list on the left.
4.  In the **"Term to search"** field, type the text you want to find (e.g., `my_global_variable`).
5.  Click on **"Search Flows in Solution"**.
6.  Review the results in the right-hand panel.

## Feedback and Bug Reports

If you find a bug or have an idea for an improvement, please [create an "Issue" here](https://github.com/TU_USUARIO_DE_GITHUB/Samcall.XTB.FlowSearch/issues).
