# ILS_teaching_demo
Project made with the purpose of demonstrating principles of Instrument landing system.
Desktop application, has 3 sections

1. Open a Instrument system demo (custom UI + graphs)

2. Open a theory file (embedded pdf file into the project)

    ​	*Note: It uses default installed pdf-viewer application, if there's none, error will occur*

3. Open a demo (embedded video file and a custom video player)

# Building

If you want to build/run project, you need **.NETFramework V4.7.2** installed on your computer. To build, simply build with Visual Studio or run `dotnet run` command.

If you have other version installed, you will have to change `<supportedRuntime>` inside *App.config* file (but you probably will need to make manual changes to source code if errors occur, so I don't recommend that).
