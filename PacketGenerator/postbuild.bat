echo XCOPY /Y PDL.xml "%2"
XCOPY /Y PDL.xml "%2"

echo cd "%2"
cd "%2"

echo START /wait PacketGenerator.exe
START /wait PacketGenerator.exe

echo XCOPY /Y GenPackets.cs "%1\DummyClient\Packet"
XCOPY /Y GenPackets.cs "%1\DummyClient\Packet"

echo XCOPY /Y GenPackets.cs "%1\Server\Packet"
XCOPY /Y GenPackets.cs "%1\Server\Packet"


