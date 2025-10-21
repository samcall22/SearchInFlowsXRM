# Search in Automate (Samcall.XTB.FlowSearch)

Un plugin para XrmToolBox que te permite buscar un término de texto específico de todos los flujos (Power Automate) que pertenecen a una solución.

## Descripción

Este plugin te ahorra horas de trabajo manual. En lugar de abrir cada flujo uno por uno para encontrar dónde se usa una variable específica, una clave de API o un texto, esta herramienta lo hace por ti, 
devolviendote la ruta de cajas hasta llegar a la caja donde esta el string.

## Características

* Carga todas las soluciones visibles de tu entorno.
* Lista todos los flujos de Power Automate que son componentes de la solución seleccionada.
* Analiza el JSON de cada flujo para encontrar un término de texto.
* Muestra los resultados agrupados por flujo.
* Indica la "ruta de cajas" (ej. `root -> Scope_Mi_Accion -> Condicion`) donde se encontró la coincidencia.

## Cómo Usarlo

1.  Abre la herramienta y conéctate a tu entorno de Dataverse.
2.  Haz clic en **"Cargar Soluciones"**.
3.  Selecciona la solución que quieres analizar en la lista de la izquierda.
4.  En el campo **"Término a buscar"**, escribe el texto que deseas encontrar (ej. `mi_variable_global`).
5.  Haz clic en **"Buscar Flujos en Solución"**.
6.  Revisa los resultados en el panel de la derecha.

## Feedback y Reporte de Errores

Si encuentras un error o tienes una idea para una mejora, por favor [crea un "Issue" aquí](https://github.com/TU_USUARIO_DE_GITHUB/Samcall.XTB.FlowSearch/issues).
