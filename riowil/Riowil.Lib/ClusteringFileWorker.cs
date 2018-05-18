using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riowil.Entities;

namespace Riowil.Lib
{
	public class ClusteringFileWorker
	{
		private const char clustersSeparator = '>';
		private const char patternValueSeparator = '-';
		private const char seriesValueSeparator = ';';
		private readonly string directory;

		public ClusteringFileWorker(string dir)
		{
			directory = dir;
		}

		public void SaveClusters(
			List<InitialCluster> clusters,
			SeriesParams seriesParams)
		{
			if (clusters == null || clusters.Count == 0)
			{
				throw new ArgumentNullException("clusters");
			}

			int[] pattern = clusters[0].ZVectors[0].Pattern; ;
			var file = CreateFileForClustes(seriesParams, pattern);

			IEnumerable<string> valuesStr = clusters.Select(cluster => cluster.ToString());
			string clustersStr = string.Join(clustersSeparator.ToString(), valuesStr);

			using (StreamWriter streamWriter = new StreamWriter(file))
			{
				streamWriter.Write(clustersStr);
			}
		}

		private FileStream CreateFileForClustes(SeriesParams seriesParams,  int[] pattern)
		{
			string clustersDirectory = GetClustersDirectory(seriesParams);
			if (!Directory.Exists(clustersDirectory))
			{
				Directory.CreateDirectory(clustersDirectory);
			}

			string fileName = PatternToFileName(pattern);
			string path = Path.Combine(clustersDirectory, fileName);

			FileStream file = new FileStream(path, FileMode.Create);
			return file;
		}

		private string GetClustersDirectory(SeriesParams seriesParams)
		{
			return GetDirectory(seriesParams, "clusters");
		}
	
		private string GetSeriesDirectory(SeriesParams seriesParams)
		{
            return Path.Combine(directory, "Series", seriesParams.Type.ToString(), seriesParams.Category);
		}

		public string GetDirectory(SeriesParams seriesParams, string subDirectory)
		{
			string dirName = string.Format(
				"{0}ftr{1}ctr{2}ft{3}ct{4}fch{5}cch{6}",
				seriesParams.Type,
				seriesParams.FirstInTraining,
				seriesParams.CountInTraining,
				seriesParams.FirstInTest,
				seriesParams.CountInTest,
				seriesParams.FistInCheck,
				seriesParams.CountInCheck);

			return Path.Combine(directory, subDirectory, dirName, seriesParams.Category);
		}

		private static string PatternToFileName(IEnumerable<int> pattern)
		{
			return string.Concat(string.Join(patternValueSeparator.ToString(), pattern), ".txt");
		}

		public IEnumerable<Series> LoadSeries(SeriesParams seriesParams)
		{
			IEnumerable<string> fileNames = GetSeriesFileNames(seriesParams);
			return fileNames.Select(fileName => LoadNormalizedSeriesFromFile(fileName, seriesParams));
		}

		private Series LoadNormalizedSeriesFromFile(string fileName, SeriesParams seriesParams)
		{
			var currentParams = new SeriesParams(seriesParams);
			currentParams.Name = fileName;

			var points = LoadSeriesListFromFile(fileName);
			List<double> normalized;

			switch (seriesParams.Type)
			{
				case SeriesType.Gold:
					normalized = MathExtended.NormilizeN(points);
					break;
				case SeriesType.OilWeek:
					normalized = 
						Math.Abs(points.Max() - points.Min())<0.00001 ?
						points.Select(x => 0.0).ToList() 
						: MathExtended.NormilizeN(points);
					currentParams.Count = normalized.Count;
					break;
				case SeriesType.Lorence:
				case SeriesType.OldLorence:
				case SeriesType.Weather:
				case SeriesType.Energy:
				default:
					normalized = MathExtended.Normilize(points);
					break;
			}

			return new Series(currentParams, normalized.GetRange(0, currentParams.Count));
		}

		private IEnumerable<string> GetSeriesFileNames(SeriesParams seriesParams)
		{
			string directoryName = GetSeriesDirectory(seriesParams);
			return Directory.GetFiles(directoryName);
		}

		private List<double> LoadSeriesListFromFile(string fileName)
		{
			string seriesString = ReadFile(fileName);
			string[] valuesStr = seriesString.Split(seriesValueSeparator);
			List<double> points = valuesStr.Select(double.Parse).ToList();

			return points;
		}

		public static string ReadFile(string fileName)
		{
			using (StreamReader sr = new StreamReader(fileName))
			{
				return sr.ReadToEnd();
			}
		}

		public IEnumerable<Template> LoadTemplatesFromInitialClusters(SeriesParams seriesParams)
		{
			string clustersDirectory = GetClustersDirectory(seriesParams);
			if (!Directory.Exists(clustersDirectory))
			{
				throw new FileNotFoundException("Clusters cannot be loaded", clustersDirectory);
			}

			string[] fileNames = Directory.GetFiles(clustersDirectory);

			if (fileNames.Length == 0)
			{
				throw new FileNotFoundException("Clusters cannot be loaded. Directory is empty", clustersDirectory);
			}

			List<Template> templates = new List<Template>(1000);
			int counter = 0;
			for (int index = 0; index < fileNames.Length; index++)
			{
				string fileName = fileNames[index];
				string clearFileName = Path.GetFileName(fileName);
				int[] pattern = FileNameToPattern(clearFileName);

				string[] clustersValuesStr = ReadFile(fileName).Split(clustersSeparator);
				List<Cluster> currentClusters =
					clustersValuesStr.Select((clusterStr, i) => InitialCluster.Parse(clusterStr, pattern).ToCluster(i + counter)).ToList();
				counter += currentClusters.Count;

				templates.Add(new Template
				{
					Id = index+1,
					Clusters = currentClusters,
					Value = pattern
				});
			}

			return templates;
		}

		private static int[] FileNameToPattern(string fileName)
		{
			string patternStr = fileName.Replace(".txt", "");
			string[] paternValuesStr = patternStr.Split(patternValueSeparator);
			int[] pattern = new int[paternValuesStr.Length];
			for (int i = 0; i < paternValuesStr.Length; i++)
			{
				pattern[i] = int.Parse(paternValuesStr[i]);
			}
			return pattern;
		}
	}
}
