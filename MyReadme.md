```bash
echo -en '*3\r\n$3\r\nSET\r\n$5\r\nmykey\r\n$7\r\nmyvalue\r\n' | nc localhost 6379
```