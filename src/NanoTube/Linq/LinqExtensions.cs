namespace NanoTube.Linq
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>	Breaks a given enumeration into specific batch sizes.. </summary>
		/// <typeparam name="T">	The Type of the enumeration. </typeparam>
		/// <param name="list">			The list to act on. </param>
		/// <param name="batchSize">	Size of the batch. </param>
		/// <returns>	Will create lazy collections of the given batch size as items are read from the IEnumerable. </returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the only way to write this")]
		public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> list, int batchSize)
		{
			var batch = new List<T>(batchSize);

			foreach (var item in list)
			{
				batch.Add(item);
				if (batch.Count == batchSize)
				{
					yield return batch;
					batch = new List<T>(batchSize);
				}
			}

			if (batch.Count > 0)
				yield return batch;
		}
	}
}