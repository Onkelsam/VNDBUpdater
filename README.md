# VNDBUpdater

VNDBUpdater can be used to manage your Visual Novels in conjunction with VNDB.org. 
A VNDB.org-account is neccessary to run the software. Login credentials are handled using the DPAPI provided by Microsoft, 
the communication between VNDBUpdater and VNDB.org is using TLS.

Once you login it will automatically get your VN-list from VNDB.org.

All changes made (like voting and setting the category) will be transmitted to VNDB.org. 

It includes the following features:

  - Add Visual Novels by name
  - Filter Visual Novels by tag
  - Index your local Visual Novels
  - Tracking the play time of Visual Novels
  - Customizeable style

The file indexer is used to automatically index your Visual Novels. You can customize which paths you wish to include and exclude. It will look for the installation directory for each Visual Novel and get its executeable. 

Tracking the play time is achieved by watching which programs get launched. If the launched program executeable path is indexed it will add to the play time of the launched Visual Novel. This means your Visual Novel must be indexed for it to work, but you don't have the start the Visual Novels from VNDBUpdater.

<b> Version </b>

2.0.6342.21181

<b> Technologies used </b>

- [Mahapps.Metro](https://github.com/MahApps/MahApps.Metro) - Used for creating Metro-styled application.
- [VndbClient](https://github.com/FredTheBarber/VndbClient) - Client for communication with VNDB. Made some changes.
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) - Client for communication with Redis.
- [Redis](http://redis.io/) - Database used.
- [WPF Notifyicon](https://bitbucket.org/hardcodet/notifyicon-wpf/src) - Minimize program to system tray.

<b> Screenshots </b>

![alt tag](http://i.imgur.com/CswtCh2.png)

![alt tag](http://i.imgur.com/Gbprnr0.png)

![alt tag](http://i.imgur.com/vwlOJEk.png)

![alt tag](http://i.imgur.com/TMnRSzx.png)
