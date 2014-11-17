MSIL Interpreter and runtime
===============

As a semestral task for a subject called Runtime Systems we had to implement a virtual machine for a subset of existing language or our own language. 
I decided to take this opportunity and learn more about MSIL and benefit from the gained knowledge in my .NET career. 
But of course, since it's basically .NET runtime running on the actual .NET runtime, it does not provide any real value besides delving into MSIL and the world of VMs. 
I could implement JVM subset instead, but I didn't consider it as much benefical for me back then.

To prove the runtime abilities we had to choose a problem like a B+ tree collection, SAT solver, etc. that we would implement in the high-level language and interpret in our VM.
There were additional requirements like implementing exceptions, heap, garbage collector and whatever we can do (threads, JIT, ...). I had no time to do some advanced stuff, not to mention optimizations.
