using System;
using System.Threading;
using System.Threading.Tasks;


namespace ExplorerX.Data {
	public interface IStorable {
		bool Save(string path);

		async ValueTask<bool> SaveAsync(string path, CancellationToken token)
			=> await Task.Run(() => Save(path), token);

		virtual async ValueTask<bool> SaveAsync(string path, double sec)
			=> await SaveAsync(path, new CancellationTokenSource(TimeSpan.FromSeconds(sec)).Token);


		bool Read(string path);

		async ValueTask<bool> ReadAsync(string path, CancellationToken token)
			=> await Task.Run(() => Read(path), token);

		virtual async ValueTask<bool> ReadAsync(string path, double sec)
			=> await ReadAsync(path, new CancellationTokenSource(TimeSpan.FromSeconds(sec)).Token);
	}
}
