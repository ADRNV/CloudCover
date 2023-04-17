# CloudCover
Autho backuper for your flash drive.
When drive connect to PC CloudCover authomaticly upload your files to cloud(while only Yandex.Disk).
Files uploads by config. This advisable when you for goting make backup or lost already 3-rd flash drive.

# Usage
For usage you need:
1. <a href="https://pl.wikipedia.org/wiki/OAuth_2.0">OAuth token of your cloud drive</a> 
2. List of dirs and filetypes
3. Add programm to autho run

All that specified in <a href="https://github.com/ADRNV/CloudCover/blob/master/CloudCover.App/Configuration/appConfig.json">appConfig.json</a>
```json
{
  "token": "OAuthToken",
  "FileDirFilter": {
    "E \\Docs": "*.docx"
  }

}
```
*Spacing after drive letter needs for correct JSON. ':' indentifies as and of key.<br>
```token``` need for executuing operations on your cloud-drive<br>
```FileDirFilter``` - specifies where and which files fetch to upload.

# How get OAuth token
1. For Yadex.Disk
    - Firstly your need <a href="https://yandex.ru/dev/id/doc/ru/register-client">create app</a> 
    - Specify permissions for created app(read and write).
    - <a href="https://yandex.ru/dev/direct/doc/start/token.html">Get OAuth token</a> 
# Whats next ?
1. Add more speed.
2. Add Google Disk
3. Add configurable cloud-drive
4. Add resotre mode from cloud-drive
