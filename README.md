# Oi™

> TortoiseGit productivity tool.

## Install

```shell
npm install -g oi-cli
```

## Usage

```console
Usage: oi COMMAND [options] [path ...]
  or   oi --version

TortoiseGit productivity tool.

Can handle "fuzzy" input and launches TortoiseGit straight from your command line.
Without showing any unnecessary alerts, and no more ALT+TAB -> explorer -> right-clicks.

Common examples:
  oi .            Opens the sync dialog
  oi a            Opens the add files dialog
  oi c -g         Opens the commit dialog, showing all files in repository
  oi l            Opens the log dialog
  oi m            Opens the merge dialog
  oi pul          Opens the pull dialog
  oi pus          Opens the push dialog
  oi reb          Opens the rebase dialog

Available options:
  -g, --global         Run command in git repository root
  -v, --verbose        1) Print debug info
                       2) Accept all commands
                       3) Show alert dialogs
  -a, --amen(d)        Enable Amend Last Commit in 'commit'
  -f, --force          Just do it - press enter in the opened window
  -m, --msg=STRING     Specify message for 'commit'
  -u, --url=URL        Specify URL for 'clone'

TortoiseGit commands:
  about                Shows the about dialog
  bisect               Allows to control the bisect logic of TortoiseGit
  branch               Opens the create branch dialog
  fetch                Opens the fetch dialog
  log                  Opens the log dialog
  clone                Opens the clone dialog
  commit               Opens the commit dialog
  add                  Adds the files in /path to version control
  revert               Reverts local modifications of a working tree
  cleanup              Cleans up the working tree in /path
  resolve              Marks a conflicted file specified in /path as resolved
  repocreate           Creates a repository in /path
  switch               Opens the switch dialog
  export               Exports a revision of the repository in /path to a zip file
  merge                Opens the merge dialog
  settings             Opens the settings dialog
  remove               Removes the file(s) in /path from version control
  rename               Renames the file in /path
  diff                 Starts the external diff program specified in the TortoiseGit settings
  showcompare          (no description)
  conflicteditor       Starts the conflict editor specified in the TortoiseGit settings
  help                 Opens the help file
  repostatus           Opens the check-for-modifications dialog
  repobrowser          Starts the repository browser dialog, pointing to the working tree given in /path
  ignore               Adds all targets in /path to the ignore list, ie
  blame                Opens TortoiseGitBlame for the file specified in /path
  cat                  Saves a file from an URL or working tree path given in /path to the location given in /savepath:path
  pull                 Opens the pull dialog in the working tree located in /path
  push                 Opens the push dialog in the working tree located in /path
  rebase               Opens the rebase dialog for the working tree located in /path
  stashsave            Opens the stash save dialog for the working tree located in /path
  stashapply           Applies to latest stash to the working tree located in /path
  stashpop             Applies to latest stash to the working tree located in /path and drops the latest stash entry
  subadd               Opens the submodule add dialog
  subupdate            Opens the submodule update dialog for and filters the submodules regarding the folder /path
  subsync              Syncs the submodule information for the working tree located in /path
  sync                 Opens the sync dialog for the working tree located in /path
  reflog               Opens the reflog dialog for the repository located in /path
  refbrowse            Opens the browse references dialog for the repository located in /path
  updatecheck          /visible: Shows the dialog even if no newer TortoiseGit version is available
  revisiongraph        Shows the revision graph for the repository given in /path
  daemon               Launches the Git Daemon for the repository given in /path
  pgpfp                Prints the TortoiseGit Release Signing Key fingerprint
  tag                  Opens the Create Tag dialog

Additional arguments to TortoiseGit can be passed using '/arg1 /arg2'.
See https://tortoisegit.org/docs/tortoisegit/tgit-automation.html for more information.
```

## Contributing

Please [report an issue](https://github.com/mortend/oi-cli/issues) if you encounter a problem, or [open a pull request](https://github.com/mortend/oi-cli/pulls) if you make a patch.

## License

MIT
