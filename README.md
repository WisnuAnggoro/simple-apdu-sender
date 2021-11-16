# simple-apdu-sender
This is a desktop app created using C# to communicate between PC and Smartcard

## How to send APDU Command
You need to create a text file containing these command:
`RST()` for resetting card (Get ATR)
`I: {APDU COMMAND}` for sending command
`O: {DATA (If Any)}{SW1SW2}` for expected result from the card (it's optional)
`'` for commenting

The following example is the script for getting UID from Mifare Classic card after getting ATR:
```
RST()
I: FFCA000000
```

And the following is an example to read a transparent EF 6F48 under DF GSM (7F20) on a SIM card
```
' Get ATR
RST()

' PIN (CHV1) is 1234
I: A02000010801020304FFFFFFFF
O: 9F17

' Select DF GSM
I: A0A40000027F20 
O: 9F17

' Select File 6F48
I: A0A40000026F48 
O: 9F10

' Read File 6F48
I: A0B0000010
```
