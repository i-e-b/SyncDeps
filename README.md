SyncDeps
========

A command-line tool to help with build dependency management

Usage:
SyncDeps "path" "src pattern" "dst pattern" ["log path"]

Path: path to check. All files matching the pattern
      in this path and sub paths will be replaced by
      the most recent copy found by exact name.

Pattern: file name pattern to match against. May use
      Wildcards '*' and '?'

Example:
      SyncDeps "C:\Projects\MyProject\" "*\bin\Debug\MyProject*.dll" "*\Dependencies\MyProject*.dll"
