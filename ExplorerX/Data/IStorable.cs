using System;
using System.Threading;
using System.Threading.Tasks;


namespace ExplorerX.Data {
	public interface IStorable {
		bool Save(string path);

		virtual async ValueTask<bool> SaveAsync(string path, CancellationToken token)
			=> await Task.Run(() => Save(path), token);

		virtual async ValueTask<bool> SaveAsync(string path, double sec)
			=> await SaveAsync(path, sec != 0
				? new CancellationTokenSource(TimeSpan.FromSeconds(sec)).Token
				: CancellationToken.None
			);


		bool Read(string path);

		virtual async ValueTask<bool> ReadAsync(string path, CancellationToken token)
			=> await Task.Run(() => Read(path), token);

		virtual async ValueTask<bool> ReadAsync(string path, double sec)
			=> await ReadAsync(path, sec != 0 
				? new CancellationTokenSource(TimeSpan.FromSeconds(sec)).Token
				: CancellationToken.None
			);
	}
}
