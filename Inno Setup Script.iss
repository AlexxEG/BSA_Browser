; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "BSA Browser"
#define MyAppVersion "1.11.0"
#define MyAppPublisher "Alexander Ellingsen"
#define MyAppURL "https://github.com/AlexxEG/BSA_Browser/"
#define MyAppExeName "BSA Browser.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{3A99EA20-5728-49D4-A05C-C870571AE6AF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
ChangesAssociations=yes
LicenseFile=LICENSE
OutputDir=Inno Output
OutputBaseFilename=BSA Browser.{#MyAppVersion}
Compression=lzma
SolidCompression=yes
UninstallDisplayIcon={app}\BSA Browser.exe
VersionInfoVersion={#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "cli"; Description: "Install CLI executable"; GroupDescription: "Optional"; Flags: unchecked
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1
Name: "bsaassociation"; Description: "Associate "".bsa"" extension"; GroupDescription: File extensions:
Name: "ba2aassociation"; Description: "Associate "".ba2"" extension"; GroupDescription: File extensions:

[Files]
Source: "Licenses\*"; DestDir: "{app}\Licenses"; Flags: ignoreversion
Source: "BSA Browser\bin\Release\BSA Browser.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "BSA Browser\bin\Release\BSA Browser.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "BSA Browser\bin\Release\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "BSA Browser\bin\Release\ICSharpCode.TextEditor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "BSA Browser\bin\Release\lz4.AnyCPU.loader.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "BSA Browser\bin\Release\Sharp.BSA.BA2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "BSA Browser\bin\Release\System.Management.Automation.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
; Optional
Source: "BSA Browser CLI\bin\Release\bsab.exe"; DestDir: "{app}"; Flags: ignoreversion; Tasks: cli

[InstallDelete]
Type: files; Name: {app}\lz4.x64.dll
Type: files; Name: {app}\lz4.x86.dll

[Registry]
Root: HKCR; Subkey: ".bsa"; ValueType: string; ValueName: ""; ValueData: "BSABrowser"; Flags: uninsdeletevalue; Tasks: bsaassociation
Root: HKCR; Subkey: ".ba2"; ValueType: string; ValueName: ""; ValueData: "BSABrowser"; Flags: uninsdeletevalue; Tasks: ba2aassociation
Root: HKCR; Subkey: "BSABrowser"; ValueType: string; ValueName: ""; ValueData: "Bethesda File Archive"; Flags: uninsdeletekey; Tasks: bsaassociation or ba2aassociation
Root: HKCR; Subkey: "BSABrowser\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"",0"; Tasks: bsaassociation or ba2aassociation
Root: HKCR; Subkey: "BSABrowser\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Tasks: bsaassociation or ba2aassociation

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

