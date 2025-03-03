echo XCOPY /Y PDL.xml "%2"
XCOPY /Y PDL.xml "%2"

echo cd "%2"
cd "%2"

echo START /wait PacketGenerator.exe
START /wait PacketGenerator.exe

echo XCOPY /Y GenPackets.cs "%1\DummyClient\Packet"
XCOPY /Y GenPackets.cs "%1\DummyClient\Packet"

echo XCOPY /Y GenPackets.cs "%1\Client\Assets\Scripts\Packet"
XCOPY /Y GenPackets.cs "%1\Client\Assets\Scripts\Packet"

echo XCOPY /Y ClientPacketManager.cs "%1\DummyClient\Packet"
XCOPY /Y ClientPacketManager.cs "%1\DummyClient\Packet"

echo XCOPY /Y ClientPacketManager.cs "%1\Client\Assets\Scripts\Packet"
XCOPY /Y ClientPacketManager.cs "%1\Client\Assets\Scripts\Packet"

echo XCOPY /Y GenPackets.cs "%1\Server\Packet"
XCOPY /Y GenPackets.cs "%1\Server\Packet"

echo XCOPY /Y ServerPacketManager.cs "%1\Server\Packet"
XCOPY /Y ServerPacketManager.cs "%1\Server\Packet"
