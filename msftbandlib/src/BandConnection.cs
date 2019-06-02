using MSFTBandLib.Command;
using MSFTBandLib.Includes;
using MSFTBandLib.Libs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MSFTBandLib {

/// <summary>
/// Microsoft Band connection class
/// </summary>
public class BandConnection<T> : BandInterface where T : class, BandSocket {

	/// <summary>Band instance</summary>
	private Band Band;

	/// <summary>Band main service socket</summary>
	private BandSocket Cargo;

	/// <summary>Band push service socket</summary>
	private BandSocket Push;


	/// <summary>
	/// Create a new connection to a given Band.
	/// 
	/// Socket instances are created for Cargo and Push using the 
	/// given socket type for the connection type, which must 
	/// implement `BandSocket`.
	/// </summary>
	public BandConnection() {
		this.Cargo = Activator.CreateInstance(
			typeof(T), new object[]{}
		) as T;
		this.Push = Activator.CreateInstance(
			typeof(T), new object[]{}
		) as T;
	}


	/// <summary>Connect to a Band.</summary>
	/// <param name="band">Band instance</param>
	/// <returns>Task</returns>
	public async Task Connect(Band band) {
		this.Band = band;
		await this.Cargo.Connect(this.Band, Band.CARGO);
		await this.Push.Connect(this.Band, Band.PUSH);
	}


	/// <summary>Disconnect all open band sockets.</summary>
	/// <returns>Task</returns>
	public async Task Disconnect() {
		await this.Cargo.Disconnect();
		await this.Push.Disconnect();
		this.Band = null;
	}


	/// <summary>
	/// Send command to the device and get response bytes.
	/// </summary>
	/// <param name="command">Command</param>
	/// <param name="args">Arguments to send</param>
	/// <param name="buffer">Receiving buffer size</param>
	/// <returns>Task<ByteArray></returns>
	public async Task<ByteArray> Command(
		Command.Command command,
		byte[] args=null, int buffer=Network.BUFFER_SIZE) {

		byte[] bytes;
		CommandPacket packet = new CommandPacket(command, args);
		bytes = await this.Cargo.SendReceive(packet.GetBytes(), buffer);
		return new ByteArray(bytes);
	}


	/// <summary>
	/// Get the Band instance.
	/// </summary>
	/// <returns>Band</returns>
	public Band GetBand() {
		return this.Band;
	}

}

}