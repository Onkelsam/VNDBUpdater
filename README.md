# VNDBUpdater

VNDBUpdater can be used to manage your Visual Novels in conjunction with VNDB.org. 
A VNDB.org-account is neccessary to run the software. Login credentials are handled using the DPAPI provided by Microsoft, 
the communication between VNDBUpdater and VNDB.org is using TLS.

Once you login it will automatically get your VN-list from VNDB.org.

All changes made (like voting and setting the category) will be transmitted to VNDB.org. 

It also includes a Tag-Filter-Mechanism and autmatic file indexer, meaning the user can set the path to the 
main directory where his VNs are installed and the program will look for the installation directory of each VN.
That is, of course, not 100% accurate. In my testing 50 of 360 VNs could not be found and had to be set manually.

<b> Version </b>

1.2.6066.41824

<b> Technologies used </b>

- [VndbClient](https://github.com/FredTheBarber/VndbClient) - Client for communication with VNDB. Made some changes.
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) - Client for communication with Redis.
- [Redis](http://redis.io/) - Database used.
- [WPF Notifyicon](https://bitbucket.org/hardcodet/notifyicon-wpf/src) - Minimize program to system tray.

<b> Screenshots </b>

![alt tag](http://i.imgur.com/BTXpprL.png)

![alt tag](http://i.imgur.com/dNCLGVO.png)

![alt tag](http://i.imgur.com/O1eOCge.png)
