---
title: "Replacing a dead RAID5 drive on an HP Microserver running Ubuntu"
categories: "raid,raid5,replacing,drive"
abstract: "A while ago I found myself in the unfortunate situation of having a degraded RAID5 array failing to boot after a blackout. So I hooked a monitor to it to see what was going on and found that it got stuck at boot time with identifying one of the disks. After a long timeout, it finally booted into Ubuntu from the attached Flash drive where it reported the degraded RAID array and dropped back to diagnostic mode. [...]"
identity: "2983,76079"
---
A while ago I found myself in the unfortunate situation of having a degraded RAID5 array failing to boot after a blackout. So I hooked a monitor to it to see what was going on and found that it got stuck at boot time with identifying one of the disks. After a long timeout, it finally booted into Ubuntu from the attached Flash drive where it reported the degraded RAID array and dropped back to diagnostic mode.

### Finding out what broke

I was pleased to find that only one of the disks was out and the superblock was in good shape, and that the rest of the system was OK:

```
admin@machine:~$ sudo mdadm --detail /dev/md0
[sudo] password for admin:
/dev/md0:
        Version : 1.2
  Creation Time : Wed May 30 19:26:25 2012
     Raid Level : raid5
     Array Size : 5860538880 (5589.05 GiB 6001.19 GB)
  Used Dev Size : 1953512960 (1863.02 GiB 2000.40 GB)
   Raid Devices : 4
  Total Devices : 3
    Persistence : Superblock is persistent

    Update Time : Thu Dec 27 13:10:16 2012
          State : clean, degraded
 Active Devices : 3
Working Devices : 3
 Failed Devices : 0
  Spare Devices : 0

         Layout : left-symmetric
     Chunk Size : 512K

           Name : machine:0  (local to host machine)
           UUID : d53b6e38:cc0ca3ae:7d3a3e69:cba8bfe1
         Events : 156429

    Number   Major   Minor   RaidDevice State
       0       8        1        0      active sync   /dev/sda1
       1       8       17        1      active sync   /dev/sdb1
       2       8       33        2      active sync   /dev/sdc1
       3       0        0        3      removed
admin@machine:~$
```

It took me a while to interpret the fact that the total number of devices was smaller than the number of Raid devices, as usually one needs to explicitly remove a failed disk from the array. Such "automatic removal" could also complicate things if there were more disks in the system, but in my case I could readily see that `/dev/sdd` was the faulty one and that indeed it was automatically dropped from the array due to the total failure to discover it at boot time.

### Doing a backup, getting a new disk

Needless to say, before I did anything else I did a full backup of the array data - this took a few days but it is definitely the way to go. In the meantime, I purchased a new disk that matched those in the system, then once the backup finished added it to the machine replacing the failed drive.

### Partitioning the new disk

With the new disk in place, I wanted to initialize it with the same partition table as the others in the array - this seemed like a convenient way to move on with all other drives having same sized partitions. So I tried copying the partition table from `/dev/sda` to the new disk:

```
sfdisk -d /dev/sda | sfdisk /dev/sdd
```

This reported back:

```
WARNING: GPT (GUID Partition Table) detected on '/dev/sda'!
The util sfdisk doesn't support GPT. Use GNU Parted.
```

Indeed, since the RAID array is above the 2TB limit for MBR partition tables it uses GPT. Luckily, I learned of `sgdisk` and did a quick `apt-get install sdisk` to install it on the machine. With this tool, I could now rephrase the previous command as:

```
admin@machine:~$ sudo sgdisk -R=/dev/sdd /dev/sda
[sudo] password for admin:
The operation has completed successfully.
admin@machine:~$ sudo sgdisk -G /dev/sdd
The operation has completed successfully.
```

The latter command is necessary to randomize the GUIDs copied from the existing partitions, apparently, this is something that RAID GPT disks require.

### Adding the new disk into the array and waiting it out

The rest of the "rescue operation" was according to the books. I added the new disk to the array:

```
mdadm --manage /dev/md0 -a /dev/sdd1
```

This started a sync with the other disks, copying data where necessary. Doing a watch to monitor progress reported:

```
admin@machine:~$ watch cat /proc/mdstat
Personalities : [linear] [multipath] [raid0] [raid1] [raid6] [raid5] [raid4] [raid10]
md0 : active raid5 sdd1[4] sdc1[2] sda1[0] sdb1[1]
      5860538880 blocks super 1.2 level 5, 512k chunk, algorithm 2 [4/3] [UUU_]
      [>....................]  recovery =  1.5% (29949252/1953512960)
      finish=500.9min speed=63994K/sec

unused devices: <none>
```

Once the sync was over, everything looked OK:

```
admin@machine:~$ cat /proc/mdstat
Personalities : [linear] [multipath] [raid0] [raid1] [raid6] [raid5] [raid4] [raid10]
md0 : active raid5 sdd1[4] sdc1[2] sda1[0] sdb1[1]
      5860538880 blocks super 1.2 level 5, 512k chunk, algorithm 2 [4/4] [UUUU]

unused devices: <none>
```

Now, if I could only find out why the `[4]` after sdd1 appears instead of `[3]`... But either way, the array seems OK and the server now starts normally. Hope this helps someone with the same problem.
