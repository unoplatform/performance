using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace DopeTestUno.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new DopeTestUno.App(), args);
		host.Run();
	}
}
}
