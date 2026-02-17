# StarBird
A simple interpreted language made in c#

fibonacci sequence:
```
var n = 10;
var a = 0;
var b = 0;
var next = b;
var count = 0;
while(count <= n){
    print(next);
    count++;
    a = b;
    b = next;
    next = a + b;
}

```