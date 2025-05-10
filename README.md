# DeMisManosGUI
Solución ASP.NET Web API
Net 9.0

Creación de un nuevo proyecto basado en otro existente

1. Crear un nuevo repo en github para la Api
2. Crear un nuevo repo en github para la Gui
3. Clonar ambos proyectos con git en tu máquina local (Git Clone Nam_Repo)
4. Copiar el contenido del proyecto base en su nuevo repo (Api) excluyendo .git y .vs	
5. Copiar el contenido del proyecto base en su nuevo repo (Gui) excluyendo .git, dist y node_modules
6. Abrir el proyecto Api, renombrar la solución y renombrar el proyecto
7. Cerrar el proyecto y abrir el archivo .sln con un bloc de notas y renombrar lo necesario
8. Abrir el proyecto y dar clic derecho sobre la solución y seleccionar Sincronizar espacios de nombres (Actualiza los namespaces)
9. Ejecutar nuevamente el proyecto y probar
10. Generar un Personal access tokens en github (foto/settings/Developer Settings/Personal access tokens/Tokens)
11. Actualizar credenciales y origin repo en GitBash con git remote set-url origin - git config user.name - git config user.email
12. Subir los cambios a GitHub
13. Actualizar Git en Visual Studio
14. Actualizar versión de net en visual studio .net en las propiedades del proyecto se sube a la versión mas reciente luego de actualizar
    visual studio .net desde el Visual Studio Installer que se busca en los programas de windows (Cuando se actualiza, trae los nuevos donets)
15. Actualizar paquetes por el Nugget Packages 




Token GitHub
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
Comando Git para hacer push a través de un token
git remote set-url origin https://carloscarmona0829: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@github.com/carloscarmona0829/SrcProjectApi.git
git config user.name "Carlos Carmona"
git config user.email "carloscarmona0829@gmail.com"
Comandos para verificar user.name y user.email
git config user.name
git config user.email
