# POSIX MQ Wrapper
Wrapper for the C POSIX Message Queues library to C#

## Compile instructions

```console
$ make
```

If you don't have make installed & don't want to bother installing it, you may also run the commands manually:

```console
$ gcc -g -Wall -fPIC -c posixwrapper.c -o posixwrapper.o
$ gcc posixwrapper.o -shared -Wall -lrt -o libposixwrapper.so
```

You'll then need to copy the files into place:

```console
$ cp libposixwrapper.so ../CS/
```

### Cross compiling

To cross compile, simply set `CC` and run make:

```console
$ CC=arm-linux-gnueabihf-gcc make clean all
```

To confirm that the file was build for ARM, run the following command:

```console
$ file libposixwrapper.so
libposixwrapper.so: ELF 32-bit LSB shared object, ARM, EABI5 version 1 (SYSV), dynamically linked, BuildID[sha1]=e928eb812c413678de8125f6731ffef6f0f145da, with debug_info, not stripped
```

As you can see, the library is compiled for 32-bit ARM.

#### Ubuntu dependencies

In a terminal, run the following commands to install the cross compiler:

```console
$ sudo apt update  # refresh package lists
$ sudo apt -y install build-essential gcc-arm-linux-gnueabihf  # install cross-compiler
```

After this, you can use the commands above to build the library.
