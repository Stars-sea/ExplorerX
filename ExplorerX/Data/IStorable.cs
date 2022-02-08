using System;
using System.Threading;
using System.Threading.Tasks;


namespace ExplorerX.Data {
	public interface IStorable {
		ValueTask<bool> SaveAsync(string path, CancellationToken token);

		virtual async ValueTask<bool> SaveAsync(string path, double sec)
			=> await SaveAsync(path, new CancellationTokenSource(TimeSpan.FromSeconds(sec)).Token);

		ValueTask<bool> ReadAsync(string path, CancellationToken token);

		virtual async ValueTask<bool> ReadAsync(string path, double sec)
			=> await ReadAsync(path, new CancellationTokenSource(TimeSpan.FromSeconds(sec)).Token);
	}
}
