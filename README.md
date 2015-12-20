# IntelliTraceEditor
Demo Application to demonstrate IntelliTrace

##Walkthrough

###Prerequisite
- Microsoft Visual Studio Enterprise 2015
- Microsoft .net Framework 4.5
- txt File

####Configure Intellitrace Events
In Visual Studio go to **Menu > Debug > Intellitrace > Open Intellitrace Settings**
Navigate to: **General > Intellitrace Events**
Add the File I/O Events.
![](https://tailf.blob.core.windows.net:443/github/IntelliTraceEditor/Settings.PNG)

###Demoscript

1. Open Solution in Microsoft Visual Studio 2015
2. Run the Program with **F5**
3. Explain the Texteditor
4. Edit Text and press Save
5. **Notice that the Texteditor lost it Focus and jumps to the start of the file**
6. Navigate to the  `private void btnSaveFile_Click(object sender, RoutedEventArgs e)` Method in MainWindow.xaml.cs at Line 154.
 - Notice the Code at Line 172:
 ``this.txtFileContents.Focus();``
 - Comment that line out:
 ``//this.txtFileContents.Focus();``
7. Run the Application again
 - **Notice the Application still has the same behaviour**
8. Run the Application again and do the walkthrough change to Visual Studio and enter the post mortem Debug modus.
![](https://tailf.blob.core.windows.net:443/github/IntelliTraceEditor/enterpostmortem.PNG)
9. Explain the Events and Calls section in Intellitrace
10. Double Click the File Save Event and use Post Mortem Debugger to step over the code.
![](https://tailf.blob.core.windows.net:443/github/IntelliTraceEditor/postdebug.PNG)
11. **Explain navigating with post mortem debugger**
12. Notice that there is a FileWatcher Changed Event in Line 94 which triggers OnFileChanged Method in Line 104:
 ``private void OnFileChanged()``
13. In Line 116 Fix the Bug by uncomment the following line:
``//if (!this.ShouldReloadFile(this.fullPath)) { return; }``


-----------------------
(c) Microsoft Corporation 2015, DevDiv and Microsoft DX