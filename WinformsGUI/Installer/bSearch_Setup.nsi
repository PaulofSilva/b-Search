;--------------------------------
; bSearch.nsi
;
; It will install bSearch files 
; into a directory that the user selects,
; and install shortcut for all users
;
;--------------------------------
!include WordFunc.nsh
!include "MUI2.nsh"
!include "FileFunc.nsh"
!include "DotNetForm.nsdinc"

!define MUI_COMPONENTSPAGE_SMALLDESC
!define MUI_LANGDLL_ALLLANGUAGES
!define INS_VERSION 4.4.6.0
!define APP_VERSION "4.4.6"

;--------------------------------
;Variables
;--------------------------------
	Var MUI_TEMP
	Var STARTMENU_FOLDER

;--------------------------------
;General
;--------------------------------
	; The name of the installer
	Name "bSearch v${APP_VERSION}"

	; The file to write
	OutFile "bSearch_Setup_v${APP_VERSION}.exe"

	; The default installation directory
	InstallDir $PROGRAMFILES\bSearch
	
	; Installer file properties
	VIProductVersion                 ${INS_VERSION}
	VIAddVersionKey ProductName      "bSearch Setup"
	VIAddVersionKey Comments         "This application and its source code are freely distributable."
	VIAddVersionKey CompanyName      "AstroComma Inc."
	VIAddVersionKey LegalCopyright   "Program and source copyright (c) AstroComma Incorporated"
	VIAddVersionKey FileDescription  "bSearch Setup"
	VIAddVersionKey FileVersion      ${INS_VERSION}
	VIAddVersionKey ProductVersion   ${INS_VERSION}
	VIAddVersionKey LegalTrademarks  "bSearch"
	VIAddVersionKey OriginalFilename "bSearch_Setup_v${APP_VERSION}.exe"

	; Registry key to check for directory (so if you install again, it will overwrite the old one automatically)
	InstallDirRegKey HKLM "Software\bSearch" "Install_Dir"
	
	;Vista redirects $SMPROGRAMS to all users without this
	RequestExecutionLevel admin
	
;--------------------------------
; Pages
;--------------------------------
	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_LICENSE "license.txt"
	Page custom fnc_DotNetForm_Show
	!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
	
	;Remember the installer language
	!define MUI_LANGDLL_REGISTRY_ROOT "HKCU" 
	!define MUI_LANGDLL_REGISTRY_KEY "Software\bSearch" 
	!define MUI_LANGDLL_REGISTRY_VALUENAME "Installer Language"
  
	;Start Menu Folder Page Configuration
	!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
	!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\bSearch" 
	!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
	!define MUI_STARTMENUPAGE_DEFAULTFOLDER "bSearch"
	!insertmacro MUI_PAGE_STARTMENU Application $STARTMENU_FOLDER

	!insertmacro MUI_PAGE_INSTFILES
	
	; finish screen settings
	!define MUI_FINISHPAGE_RUN $INSTDIR\bSearch.exe
	!define MUI_FINISHPAGE_RUN_NOTCHECKED
	!insertmacro MUI_PAGE_FINISH

	; uninstaller pages
	!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_INSTFILES
	
;--------------------------------
;Language
;--------------------------------
	; First is default
	!insertmacro MUI_LANGUAGE "English"
	!insertmacro MUI_LANGUAGE "Danish"
	!insertmacro MUI_LANGUAGE "German"
	!insertmacro MUI_LANGUAGE "Italian"
	!insertmacro MUI_LANGUAGE "Spanish"
	!insertmacro MUI_LANGUAGE "Polish"
	!insertmacro MUI_LANGUAGE "French"
	
	LangString MessageDotNetRequires ${LANG_ENGLISH} "Microsoft .NET Framework v4.0 or newer is required."
	LangString MessageDotNetRequires ${LANG_DANISH} "Microsoft NET Framework v4.0 eller nyere er påkrævet."
	LangString MessageDotNetRequires ${LANG_GERMAN} "Microsoft NET Framework v4.0 oder höher erforderlich."
	LangString MessageDotNetRequires ${LANG_ITALIAN} "È richiesto Microsoft NET Framework v4.0 o più recente."
	LangString MessageDotNetRequires ${LANG_SPANISH} "Se requiere Microsoft NET Framework v4.0 o posterior."
	LangString MessageDotNetRequires ${LANG_POLISH} "Wymagany Microsoft .NET Framework v4.0 lub nowszy."
	LangString MessageDotNetRequires ${LANG_FRENCH} "Microsoft .NET Framework v4.0 ou plus récent est requis."
	
	LangString MessageDotNetChecking ${LANG_ENGLISH} "Checking for version 4.0 or newer..."
	LangString MessageDotNetChecking ${LANG_DANISH} "Kontrol for version 4.0 eller nyere ..."
	LangString MessageDotNetChecking ${LANG_GERMAN} "Überprüfen auf Version 4.0 oder neuer ..."
	LangString MessageDotNetChecking ${LANG_ITALIAN} "Controllo versione 4.0 o più recente ..."
	LangString MessageDotNetChecking ${LANG_SPANISH} "Comprobación de la versión 4.0 o más reciente ..."
	LangString MessageDotNetChecking ${LANG_POLISH} "Sprawdzanie dostępności wersji 4.0 lub nowszej..."
	LangString MessageDotNetChecking ${LANG_FRENCH} "Vérification de la version 4.0 ou plus récente ..."
	
	LangString MessageDotNetFound ${LANG_ENGLISH} "v4.0 or newer found."
	LangString MessageDotNetFound ${LANG_DANISH} "v4.0 eller nyere fundet."
	LangString MessageDotNetFound ${LANG_GERMAN} "v4.0 oder höher gefunden."
	LangString MessageDotNetFound ${LANG_ITALIAN} "v4.0 o più recente trovato."
	LangString MessageDotNetFound ${LANG_SPANISH} "v4.0 o más reciente encontrado."
	LangString MessageDotNetFound ${LANG_POLISH} "Znaleziono wersję 4.0 lub nowszą."
	LangString MessageDotNetFound ${LANG_FRENCH} "V4.0 ou plus récente."
	
	LangString MessageDotNetNotFound ${LANG_ENGLISH} "Microsoft .NET Framework is not installed. Downloading..."
	LangString MessageDotNetNotFound ${LANG_DANISH} "Microsoft NET Framework er ikke installeret. Downloading..."
	LangString MessageDotNetNotFound ${LANG_GERMAN} "Microsoft NET Framework ist nicht installiert. Herunterladen..."
	LangString MessageDotNetNotFound ${LANG_ITALIAN} "Microsoft NET Framework non è installato. Download..."
	LangString MessageDotNetNotFound ${LANG_SPANISH} "Microsoft NET Framework no está instalado. Descarga..."
	LangString MessageDotNetNotFound ${LANG_POLISH} "Microsoft .NET Framework nie jest zainstalowany. Pobieranie..."
	LangString MessageDotNetNotFound ${LANG_FRENCH} "Microsoft .NET Framework n'est pas installé. Téléchargement..."
	
	LangString MessageDotNetDownloading ${LANG_ENGLISH} "Downloading from Microsoft..."
	LangString MessageDotNetDownloading ${LANG_DANISH} "Downloading fra Microsoft ..."
	LangString MessageDotNetDownloading ${LANG_GERMAN} "Herunterladen von Microsoft ..."
	LangString MessageDotNetDownloading ${LANG_ITALIAN} "Download da Microsoft ..."
	LangString MessageDotNetDownloading ${LANG_SPANISH} "Descarga de Microsoft ..."
	LangString MessageDotNetDownloading ${LANG_POLISH} "Pobieranie od Microsoft..."
	LangString MessageDotNetDownloading ${LANG_FRENCH} "Téléchargement de Microsoft ..."
	
	LangString MessageDotNetInstalling ${LANG_ENGLISH} "Download successful, installing..."
	LangString MessageDotNetInstalling ${LANG_DANISH} "Hente en succes, installation ..."
	LangString MessageDotNetInstalling ${LANG_GERMAN} "Erfolgreichen Herunterladen, der Installation von ..."
	LangString MessageDotNetInstalling ${LANG_ITALIAN} "Scarica di successo, l'installazione ..."
	LangString MessageDotNetInstalling ${LANG_SPANISH} "Descarga éxito, la instalación de ..."
	LangString MessageDotNetInstalling ${LANG_POLISH} "Pobieranie zakończone, instalowanie..."
	LangString MessageDotNetInstalling ${LANG_FRENCH} "Télécharger réussie, installer ..."
	
	LangString MessageDotNetSuccess ${LANG_ENGLISH} "Installation completed."
	LangString MessageDotNetSuccess ${LANG_DANISH} "Installation afsluttet."
	LangString MessageDotNetSuccess ${LANG_GERMAN} "Installation abgeschlossen."
	LangString MessageDotNetSuccess ${LANG_ITALIAN} "L'installazione completata."
	LangString MessageDotNetSuccess ${LANG_SPANISH} "Completó la instalación."
	LangString MessageDotNetSuccess ${LANG_POLISH} "Instalacja zakończona."
	LangString MessageDotNetSuccess ${LANG_FRENCH} "Installation complétée."
	
	LangString MessageDotNetFailure ${LANG_ENGLISH} "Unable to download .NET Framework.  bSearch can be installed, but will not function without the Framework!"
	LangString MessageDotNetFailure ${LANG_DANISH} "Kan ikke downloade .NET Framework. bSearch kan installeres, men vil ikke fungere uden ramme!"
	LangString MessageDotNetFailure ${LANG_GERMAN} "Können Sie .NET Framework herunterladen. bSearch installiert werden kann, aber nicht ohne Rahmen funktionieren!"
	LangString MessageDotNetFailure ${LANG_ITALIAN} "Impossibile scaricare .NET Framework. bSearch può essere installato, ma non funziona senza il quadro!"
	LangString MessageDotNetFailure ${LANG_SPANISH} "No se puede descargar .NET Framework. bSearch se puede instalar, pero no funcionará sin el Marco!"
	LangString MessageDotNetFailure ${LANG_POLISH} "Nie można pobrać .NET Framework. bSearch może zostać zainstalowany, ale nie będzie działać bez tej biblioteki!"
	LangString MessageDotNetFailure ${LANG_FRENCH} "Impossible de télécharger .NET Framework. bSearch peut être installé, mais ne fonctionnera pas sans le Framework!"
	
	LangString TITLE_SecRequired ${LANG_ENGLISH} "bSearch (required)"
	LangString TITLE_SecRequired ${LANG_DANISH} "bSearch (påkrævet)"
	LangString TITLE_SecRequired ${LANG_GERMAN} "bSearch (erforderlich)"
	LangString TITLE_SecRequired ${LANG_ITALIAN} "bSearch (richiesto)"
	LangString TITLE_SecRequired ${LANG_SPANISH} "bSearch (requerida)"
	LangString TITLE_SecRequired ${LANG_POLISH} "bSearch (wymagane)"
	LangString TITLE_SecRequired ${LANG_FRENCH} "bSearch (requis)"
	
	LangString DESC_SecRequired ${LANG_ENGLISH} "The bSearch program and its required files."
	LangString DESC_SecRequired ${LANG_DANISH} "Det bSearch programmet og dets nødvendige filer."
	LangString DESC_SecRequired ${LANG_GERMAN} "Die bSearch Programm und seine benötigten Dateien."
	LangString DESC_SecRequired ${LANG_ITALIAN} "Il programma bSearch ei file necessari."
	LangString DESC_SecRequired ${LANG_SPANISH} "El programa bSearch y sus archivos requeridos."
	LangString DESC_SecRequired ${LANG_POLISH} "Program bSearch oraz wymagane przez niego pliki."
	LangString DESC_SecRequired ${LANG_FRENCH} "Le programme bSearch et ses fichiers requis."
	
	LangString TITLE_SecFolderSearch ${LANG_ENGLISH} "Explorer Context Menu"
	LangString TITLE_SecFolderSearch ${LANG_DANISH} "Stifinder Context Menu"
	LangString TITLE_SecFolderSearch ${LANG_GERMAN} "Explorer-Kontextmenü"
	LangString TITLE_SecFolderSearch ${LANG_ITALIAN} "Context Menu Explorer"
	LangString TITLE_SecFolderSearch ${LANG_SPANISH} "Explorador de Menú contextual"
	LangString TITLE_SecFolderSearch ${LANG_POLISH} "Menu kontekstowe Eksplorera"
	LangString TITLE_SecFolderSearch ${LANG_FRENCH} "Menu contextuel de l'explorateur"
	
	LangString DESC_SecFolderSearch ${LANG_ENGLISH} "Creates a shortcut to search a folder when right-clicking them."
	LangString DESC_SecFolderSearch ${LANG_DANISH} "Anvend sgning ved hjreklik p mapper."
	LangString DESC_SecFolderSearch ${LANG_GERMAN} "Auf Rechtsklick Ordnereinstellungen ffnen."
	LangString DESC_SecFolderSearch ${LANG_ITALIAN} "Aggiungi l&apos;opzione di ricerca al menu contestuale di Esplora Risorse."
	LangString DESC_SecFolderSearch ${LANG_SPANISH} "Fijar la opcin del derecho-tecleo en carpetas."
	LangString DESC_SecFolderSearch ${LANG_POLISH} "Tworzy skrót do przeszukiwania folderu po naciśnięciu na nim prawym klawiszem myszy."
	LangString DESC_SecFolderSearch ${LANG_FRENCH} "Crée un raccourci pour rechercher un dossier lorsque vous cliquez avec le bouton droit de la souris."
	
	LangString TITLE_SecDesktopShortcut ${LANG_ENGLISH} "Desktop Shortcut"
	LangString TITLE_SecDesktopShortcut ${LANG_DANISH} "Skrivebords Genvej."
	LangString TITLE_SecDesktopShortcut ${LANG_GERMAN} "Verknpfung auf dem Desktop."
	LangString TITLE_SecDesktopShortcut ${LANG_ITALIAN} "Collegamenti sul desktop."
	LangString TITLE_SecDesktopShortcut ${LANG_SPANISH} "Atajo de escritorio."
	LangString TITLE_SecDesktopShortcut ${LANG_POLISH} "Skrót na pulpicie"
	LangString TITLE_SecDesktopShortcut ${LANG_FRENCH} "Raccourci de bureau"
	
	LangString DESC_SecDesktopShortcut ${LANG_ENGLISH} "Creates a shortcut on the desktop for bSearch."
	LangString DESC_SecDesktopShortcut ${LANG_DANISH} "Skrivebords Genvej."
	LangString DESC_SecDesktopShortcut ${LANG_GERMAN} "Verknpfung auf dem Desktop."
	LangString DESC_SecDesktopShortcut ${LANG_ITALIAN} "Collegamenti sul desktop."
	LangString DESC_SecDesktopShortcut ${LANG_SPANISH} "Atajo de escritorio."
	LangString DESC_SecDesktopShortcut ${LANG_POLISH} "Tworzy skrót do bSearch na pulpicie."
	LangString DESC_SecDesktopShortcut ${LANG_FRENCH} "Crée un raccourci sur le bureau pour bSearch."
	
	LangString RemoveText ${LANG_ENGLISH} "Remove: "
	LangString RemoveText ${LANG_DANISH} "Fjern: "
	LangString RemoveText ${LANG_GERMAN} "Entfernen: "
	LangString RemoveText ${LANG_ITALIAN} "Rimuovere: "
	LangString RemoveText ${LANG_SPANISH} "Eliminar: "
	LangString RemoveText ${LANG_POLISH} "Usuń: "
	LangString RemoveText ${LANG_FRENCH} "Retirer: "
	
	;If you are using solid compression, files that are required before
	;the actual installation should be stored first in the data block,
	;because this will make your installer start faster.
	!insertmacro MUI_RESERVEFILE_LANGDLL
  
;--------------------------------
;Handles checking for .Net framework 
; v4.0, aborts install if not found
;--------------------------------
Function .onInit
	!insertmacro MUI_LANGDLL_DISPLAY
	
	;Call IsDotNETVersion4Installed
	;Pop $1

	;${If} $1 != 1
	;	MessageBox MB_OK|MB_ICONSTOP $(MessageDotNetBadVersion)
	;	Abort
	;${EndIf}
	
FunctionEnd
	
;--------------------------------
; Required Files
;--------------------------------
Section $(TITLE_SecRequired) SecRequired

  SectionIn RO
  
  SetShellVarContext all
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put main files
  File "..\bin\release\bSearch.exe"
  File "..\bin\release\bSearch.exe.config"
  File "..\bin\release\bSearch.Common.dll"
  File "..\bin\release\libbSearch.dll"
  File "..\bin\release\bSearch.AdminProcess.exe"
  File "..\bin\release\bSearch.AdminProcess.exe.config"
  File "..\bin\release\ICSharpCode.AvalonEdit.dll"
  File "..\bin\release\NLog.dll"
  File "..\bin\release\bSearch.VisualElementsManifest.xml"
  File "..\bin\release\bSearch_256x256.png"
  File "license.txt"
  File "readme.txt"
  
  ;Store installation folder
  WriteRegStr HKCU "Software\bSearch" "" $INSTDIR
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "DisplayName" "bSearch"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "DisplayIcon" '"$INSTDIR\bSearch.exe",0'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "Publisher" "AstroComma, Inc."
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "DisplayVersion" "${APP_VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "NoRepair" 1
  
  ;Size for uninstall
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch" "EstimatedSize" "$0"
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ;Create start menu shortcuts
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
		CreateDirectory "$SMPROGRAMS\$STARTMENU_FOLDER"
		CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\bSearch.lnk" "$INSTDIR\bSearch.exe"
		;CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\bSearch Help.lnk" "$INSTDIR\bSearch-Help.chm"
		CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall bSearch.lnk" "$INSTDIR\Uninstall.exe"
  !insertmacro MUI_STARTMENU_WRITE_END
  
  ;Associate .agproj files with this application
  ;Call AssociateProjectFile
 
SectionEnd

;--------------------------------
;The Right-Click Folder Search Section 
;	Optional section (can be disabled by the user)
;--------------------------------
Section /o $(TITLE_SecFolderSearch) SecFolderSearch

	WriteRegStr HKCR "Directory\shell\bSearch" "" "Search using &bSearch..."
	WriteRegStr HKCR "Directory\Background\shell\bSearch" "" "Search using &bSearch..."
	WriteRegStr HKCR "Drive\shell\bSearch" "" "Search using &bSearch..."
	
	; shows icon in Windows 7+
	WriteRegStr HKCR "Directory\shell\bSearch" "Icon" '"$INSTDIR\bSearch.exe",0'
	WriteRegStr HKCR "Directory\Background\shell\bSearch" "Icon" '"$INSTDIR\bSearch.exe",0'
	WriteRegStr HKCR "Drive\shell\bSearch" "Icon" '"$INSTDIR\bSearch.exe",0'
	
	WriteRegStr HKCR "Directory\shell\bSearch\command" "" '"$INSTDIR\bSearch.exe" "%L"'
	WriteRegStr HKCR "Directory\Background\shell\bSearch\command" "" '"$INSTDIR\bSearch.exe" "%V"'
	WriteRegStr HKCR "Drive\shell\bSearch\command" "" '"$INSTDIR\bSearch.exe" "%L"'

	DetailPrint "Create shortcut: Right-Click folder search"
	
SectionEnd

;--------------------------------
;The Desktop shortcut Section 
;	Optional section (can be disabled by the user)
;--------------------------------
Section /o $(TITLE_SecDesktopShortcut) SecDesktopShortcut
	CreateShortCut "$DESKTOP\bSearch.lnk" "$INSTDIR\bSearch.exe"
SectionEnd

;--------------------------------
;Assign language strings to sections
;--------------------------------
	!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
		!insertmacro MUI_DESCRIPTION_TEXT ${SecRequired} $(DESC_SecRequired)
		!insertmacro MUI_DESCRIPTION_TEXT ${SecFolderSearch} $(DESC_SecFolderSearch)
		!insertmacro MUI_DESCRIPTION_TEXT ${SecDesktopShortcut} $(DESC_SecDesktopShortcut)
	!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
; Uninstaller
;--------------------------------
Section "Uninstall"

	SetShellVarContext all
	
	; Remove files and uninstaller
    Delete $INSTDIR\license.txt
	Delete $INSTDIR\readme.txt
	Delete $INSTDIR\ICSharpCode.AvalonEdit.dll
	Delete $INSTDIR\NLog.dll
	Delete $INSTDIR\bSearch.AdminProcess.exe.config
	Delete $INSTDIR\bSearch.AdminProcess.exe
	Delete $INSTDIR\libbSearch.dll
	Delete $INSTDIR\bSearch.Common.dll
	Delete $INSTDIR\bSearch.exe.config
	Delete $INSTDIR\bSearch.exe
	Delete $INSTDIR\bSearch.VisualElementsManifest.xml
	Delete $INSTDIR\bSearch_256x256.png
	Delete "$INSTDIR\Uninstall.exe"

	; Remove directories used
	;RMDir "$INSTDIR\Language"
	;RMDir "$INSTDIR\Plugins"
	RMDir "$INSTDIR"
	
	; remove registry settings
	;Call un.AssociateProjectFile
	Call un.RemoveFolderSearch
	
	; Remove Shortcuts
	Delete "$DESKTOP\bSearch.lnk"
	
	; Remove Start Menu entries
	!insertmacro MUI_STARTMENU_GETFOLDER Application $MUI_TEMP
	
	Delete "$SMPROGRAMS\$MUI_TEMP\bSearch.lnk"
	;Delete "$SMPROGRAMS\$MUI_TEMP\bSearch Help.lnk"
	Delete "$SMPROGRAMS\$MUI_TEMP\Uninstall bSearch.lnk"

	;Delete empty start menu parent diretories
	StrCpy $MUI_TEMP "$SMPROGRAMS\$MUI_TEMP"

	startMenuDeleteLoop:
		ClearErrors
		RMDir $MUI_TEMP
		GetFullPathName $MUI_TEMP "$MUI_TEMP\.."

		IfErrors startMenuDeleteLoopDone

		StrCmp $MUI_TEMP $SMPROGRAMS startMenuDeleteLoopDone startMenuDeleteLoop
	startMenuDeleteLoopDone:

	; delete registry entries
	DeleteRegKey HKCU "Software\bSearch"
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bSearch"
	DeleteRegKey HKLM "Software\bSearch"
	
SectionEnd

;--------------------------------
;determines .net4 installed
;--------------------------------
;Function IsDotNETVersion4Installed
;
;	Push $1
;		
;	ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" Install
;	IntOp $1 $0 & 1
;
;	Exch $1
;	
;FunctionEnd

;--------------------------------
;associates .agproj files with 
;  this application
;--------------------------------
;Function AssociateProjectFile
	
;	WriteRegStr HKCR ".agproj" "" "agprojfile"
	
;	WriteRegStr HKCR "agprojfile" "" "bSearch Project File"
;	WriteRegDWORD HKCR "agprojfile" "EditFlags" 0
;	WriteRegStr HKCR "agprojfile\DefaultIcon" "" '"$INSTDIR\bSearch.exe",0'
;	WriteRegStr HKCR "agprojfile\shell\open\command" "" '"$INSTDIR\bSearch.exe" "%1"'
	
;	DetailPrint "Create association: .agproj files"
	
;FunctionEnd

;--------------------------------
;Removes association of .agproj 
;  files with this application
;--------------------------------
;Function un.AssociateProjectFile
	
;	DeleteRegKey HKCR ".agproj"
;	DeleteRegKey HKCR "agprojfile"
	
;	DetailPrint "Remove association: .agproj files"
	
;FunctionEnd

;--------------------------------
;Removes Right-Click Folder 
;  Search
;--------------------------------
Function un.RemoveFolderSearch
	
	DeleteRegKey HKCR "Directory\shell\bSearch"
	DeleteRegKey HKCR "Directory\Background\shell\bSearch"
	DeleteRegKey HKCR "Drive\shell\bSearch"
	
	DetailPrint "$(RemoveText)$(TITLE_SecFolderSearch)"
	
FunctionEnd

;--------------------------------
;Uninstaller Functions
;--------------------------------
Function un.onInit

	!insertmacro MUI_UNGETLANGUAGE
  
FunctionEnd