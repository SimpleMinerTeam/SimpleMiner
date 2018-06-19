 Simple Miner for both CPU & GPU mining
=============================

Simple Miner is a hight performance Cryptonight miner program for both CPU & GPU mining. Our team is made it for simple using without editing config- or batch files.
This miner is free-to-use, however current developer fee is 1.0%.
Our program based on XMRig miner.

VERSION HISTORY
---------------
v2.1.1.0 (2018/06/19)
Bugfix:
 - Electroneum fork support
 - Decrease CPU usage when Nvidia GPU is in use
 - Minor bug fixes

v2.1.0.0 (2018/05/18)
Improvements:
 - IPBC support added
 - Updated to the latest Xmrig version
 - Windows optimization:
	* UAC control
	* Defender Real-time protection control
	* Defender SmartScreen control
	* Sleep mode control
	* Windows update control
	* AMD compute mode switching

Bugfix:
 - Autostart did not work properly in certain cases
 - Improved stratum handling to avoid stucked connection

v2.0.6.0 (2018/04/25)
Improvements:
 - Stellite (XTL) support

Bugfix:
 - Case of unstable network connection, Simple Miner may have closed
 - Too many rejected shares case of using Cryptonight V7 algorithm with NiceHash

v2.0.5.0 (2018/04/20)
Improvements:
 - Cryptonight-Heavy support
 - Cryptonight-Lite support
 - Algorithm selection ability for each pool

v2.0.4.1 (2018/04/14)
Improvements:
 - Graft hardfork support

v2.0.4.0 (2018/04/11)
Improvements:
 - User interface branding
 - More transparent logging
 - Pool url check (to avoid invalid pool address)
 - Updated the latest Xmrig version
 - Graft coin support

Bugfix:
 - GPU mining crashed during autostart
 - Fix for GPU mining issue when pool address provided as IP

v2.0.3.0 (2018/04/04)
Improvements:
- Monero ASIC resistance algorithm support
- AMD & Nvidia GPU mining support
- Updated to latest Xmrig version
- Pool storage converted to XML format

v1.3.0.0 (2018/03/09)
Improvements: 
 - Silent mode support
 
Bugfix:
 - Remove unnecessary spaces from pool managing data. (Pool address, wallet address...etc)
 - Fix for an autostart issue which prevent application to start properly.
 
v1.2.0.0 (2018/02/23)
Improvements: 
 - Updated to latest Xmrig version
 
Bugfix:
 - Failover pool sequence issue
 - Unable to scrolling pool list
 - Crashed when CPU affinity was modified on Windows Server 2016
 - Occasional collapsing on Windows 7
 - Windows Server 2012 compatibility problem
 
v1.1.0.0 (2018/01/21)
 New features:
 - Pool managing
 - Failover support
 
 Bugfix:
 - Popup web browser window when internet connection lost

v1.0.0.0 (2017/11/28)
 - Initial release

SYSTEM REQUIREMENTS
-------------------
Windows 7/8/8.1/10/Server 2012 R2/Server 2016 x64 operation system


FEATURES
--------
* High performance

* Easy to use

* No need installation

* AMD & Nvidia GPU mining support

* No need manually editing batch files or config files

* Pool management

* Failover support

* Silent mode support

* Monero ASIC resistance algorithm support

* Supporting all Cryptonight based coins (Monero, Electroneum, Sumokoin, etc.)

* Help to generate optimal CPU affinity

* Set to maximum CPU usage (when number of CPU thread option is auto)

* Virus and malware free

* Nicehash support



OPTIONS
-------
[Pool address]: URL of mining server (e.g. etnpool.sytes.net). Only Stratum protocol is supported.

[Port]: The port number of the pool (e.g. 3333)

[Wallet address]: Your wallet address

[Password]: Password for the pool. (default password is "x")

[Number of CPU threads]: When CPU is mining how many threads use. 0 value means using auto setting.

[Maximum CPU usage]: It is only available when the number of CPU threads option is 0 (auto).

[CPU affinity]: Set process affinity to CPU core(s). You can click on the auto config icon to determine what option is suit for you.

[Autostart mining]: Starts mining after launch Simple Miner.

[Starting delay (sec)]: Delay for Autostart mining option. It is useful when the miner program starts with Windows.

[Launch on Windows startup]: Simple Miner starts with Windows. (Don't forget turn on Autostart mining option with this!)

[Nicehash support]: Enable nicehash support if you wish mining into a Nicehash pool.

[Minimize to tray]: Simple Miner starts minimized.

[Log to a file]: The Simple Miner makes a log file into its directory.



MAXIMUM PERFORMANCE CHECKLIST
-----------------------------
* Idle operating system

* Optimal thread count

* Use modern CPUs with AES-NI instructuon set

* Try setup optimal cpu affinity

* Enable fast memory (Large/Huge pages) (More details: https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/enable-the-lock-pages-in-memory-option-windows)


CPU MINING PERFORMANCE
----------------------
AMD FX 8150 - 344 H/s (Number of threads: 7)

AMD FX 8300 - 354 H/s (Number of threads: 7)

Intel i5-4690 - 234 H/s (Number of threads: auto)

Intel 2x Xeon e5-2420 - 585 H/s (Number of threads: 12)

AMD Ryzen 1500x - 433 H/s (Number of threads: auto)


CONTACT
-------
If you would like to share your opinion, idea or remark, feel free contact us! 
Our e-mail address: simpleminerteam@gmail.com


DONATIONS
--------
We have a lot of idea for improve this project... so if you have some spread coins please support us! ;o)

BTC: 1LXs5VK16TgUYebuP4YDPUZLrwXDdKZM32

ETH: 0xc072ad85a54ec1591a54dbf0d54da09971b0ccb2

LTC: LZPss7iBAdJTYbJBThKnXrC47mA2WgF24B

Dash: Xh1HY1gJTxoH2qfV8hVxH6xMhcSh815h2u

XMR: 4BrL51JCc9NGQ71kWhnYoDRffsDZy7m1HUU7MRU4nUMXAHNFBEJhkTZV9HdaL4gfuNBxLPc3BeMkLGaPbF5vWtANQrGBNaWKx1HPVM1Y7t

ETN: etnkEKwVnTfcwuBnSKuQgaQetJ7SiqnH3c6TU1HXBgFkSrtwaviEkBijMVrMhGi1aP4hPKJwaaKp5Rqhxi4pyP9i26A9dRJEhW

ZEC: t1RQjurGyA3f2w7W7zL1zunZ9YrDQhsWnaR

SUMO: Sumoo2rnbqd1QtcNsu15T18VTr96wpHPuECGFEsn39pEXj6zPhZA5CWZdPJA5PHWz64akcbi184QZG9ago6no9ZvZbrwwxk8E6b