# Project Context: IndaloAventurAPP

IndaloAventurApp es un aplicación del club de montañismo indaloaventura que podrá ser accedida por web o instalada desde el navegador en un dispositivo movil.La App permitirá realizar todas las operaciones que expone el API IndaloaventurAPI que se ubica en el mismo repositorio pero una carpeta por encima de este proyecto

La propuesta para que sea escalable es la siguiente
Blazor Web App + InteractiveWebAssembly + PWA + Shared Components


## Stack
- Language: C#
- Runtime: .Net 9
- Validation: FluentValidation or DataAnnotations
- Tailwind 4.3

## Estructura principal
- IndaloaventurApp.SharedUI      ← componentes Razor
- IndaloaventurApp.Web           ← aplicación web app Blazor + PWA

## Requisitos del sistema
- Crea una estructura desacoplada, por eso el proyecto SharedUI, que me permita en un futuro reutilizar los componentes compartidos Blazor para desarrollar aplicaciones hibridas o movil usando esos mismos componentes, como una app hybrida en blazor. 
- Creamos un archivo de recursos para los literales de la aplicación y usamos inyeccion de dependencias para usar localizer. Los literales se identificaran por claves cortas que hagan referencia al texto que identifican. Por ahora solo usamos idioma Español
- Todos los estilos se definen usando .scss partiendo de un fichero global style.scss que haga referencia al resto de ficheros de forma organizada (EJ: components.scss, etc..), los estilos NUNCA se definen inline o a nivel de componente, siempre se definen de forma organizada en ficheros scss.
- Cuando se cree un componente razor, siempre se genera el codigo C# en una clase partial separada excepto codigo que deba incluirse para definir logica de la vista
- Ya se dispone de entorno de api, esta ubicado en una carpeta superior a este proyecto, pero dentro del mismo repo, la app web se basara en los endpoint ofrecidos por el API. El archivo json con la coleccion de endpoint es "endpoints.json" 

##




