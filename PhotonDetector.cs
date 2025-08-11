using System;

namespace NoiseSimulator
{
    /// <summary>
    /// Photon detection simulator for uniform random photon flux
    /// </summary>
    public class PhotonDetector
    {
        private readonly Random random;
        private double readNoise;

        public PhotonDetector(int? seed = null, double readNoise = 0.0)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
            this.readNoise = readNoise;
        }

        /// <summary>
        /// Gets or sets the additive read noise level
        /// </summary>
        public double ReadNoise
        {
            get => readNoise;
            set => readNoise = Math.Max(0.0, value); // Ensure non-negative
        }

        /// <summary>
        /// Gets the offset value used to compensate for read noise bias
        /// </summary>
        public int Offset
        {
            get => (int)Math.Round(7 * readNoise);
        }

        /// <summary>
        /// Generates the number of detected photons during time P seconds
        /// for a uniform random flux averaging X photons per second.
        /// 
        /// Mathematical background:
        /// - Photon arrivals follow a Poisson process
        /// - The number of photons in time P follows Poisson distribution with λ = X × P
        /// - P(k) = (λ^k × e^(-λ)) / k! where k is the number of detected photons
        /// 
        /// The Poisson process properties:
        /// 1. Independence: Photon arrivals are independent of each other
        /// 2. Stationarity: The probability of arrival is constant over time
        /// 3. Memorylessness: The time until the next photon is independent of past arrivals
        /// 
        /// Read noise is added as additive Gaussian noise to simulate detector readout noise.
        /// </summary>
        /// <param name="averagePhotonsPerSecond">X - average photons per second</param>
        /// <param name="detectionTimeSeconds">P - detection time in seconds</param>
        /// <returns>Number of detected photons (including read noise)</returns>
        public int GeneratePhotonDetection(double averagePhotonsPerSecond, double detectionTimeSeconds)
        {
            // Calculate the Poisson parameter λ = X × P
            double lambda = averagePhotonsPerSecond * detectionTimeSeconds;

            int photonCount = 0;
            
            if (lambda > 0)
            {
                // For small lambda values (λ < 30), use direct method
                if (lambda < 30)
                {
                    // Direct method: generate exponential inter-arrival times
                    // This is equivalent to counting Poisson events
                    double L = Math.Exp(-lambda);
                    int k = 0;
                    double p = 1.0;
                    
                    do
                    {
                        k++;
                        p *= random.NextDouble();
                    } while (p > L);
                    
                    photonCount = k - 1;
                }
                else
                {
                    // For larger lambda values, use normal approximation
                    // Poisson(λ) ≈ Normal(λ, √λ) for λ > 30
                    // This is based on the Central Limit Theorem
                    double mean = lambda;
                    double stdDev = Math.Sqrt(lambda);
                    
                    // Box-Muller transform to generate normal distribution
                    double u1 = random.NextDouble();
                    double u2 = random.NextDouble();
                    
                    double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
                    double normalValue = mean + stdDev * z0;
                    
                    // Convert to integer and ensure non-negative
                    photonCount = (int)Math.Round(normalValue);
                    photonCount = Math.Max(0, photonCount);
                }
            }
            
            // Add read noise if specified
            if (readNoise > 0)
            {
                // Add offset in ADUs to prevent read noise clipping
                photonCount += Offset;

                // Generate Gaussian read noise
                double u1 = random.NextDouble();
                double u2 = random.NextDouble();
                
                double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
                double readNoiseValue = readNoise * z0;
                
                // Add read noise to the analog signal before quantization
                // This preserves the statistical properties while maintaining physical realism
                double totalSignal = photonCount + readNoiseValue;
                return Math.Max(0, (int)Math.Round(totalSignal));
            }
            
            return photonCount;
        }

        /// <summary>
        /// Generates multiple photon detection measurements
        /// </summary>
        /// <param name="averagePhotonsPerSecond">X - average photons per second</param>
        /// <param name="detectionTimeSeconds">P - detection time in seconds</param>
        /// <param name="numMeasurements">Number of measurements to generate</param>
        /// <returns>Array of detected photon counts</returns>
        public int[] GenerateMultipleDetections(double averagePhotonsPerSecond, double detectionTimeSeconds, int numMeasurements)
        {
            int[] results = new int[numMeasurements];
            
            for (int i = 0; i < numMeasurements; i++)
            {
                results[i] = GeneratePhotonDetection(averagePhotonsPerSecond, detectionTimeSeconds);
            }
            
            return results;
        }

        /// <summary>
        /// Calculates theoretical statistics for the photon detection (after offset subtraction and exposure averaging)
        /// </summary>
        /// <param name="averagePhotonsPerSecond">X - average photons per second</param>
        /// <param name="detectionTimeSeconds">P - detection time in seconds</param>
        /// <param name="numberOfExposures">Number of exposures to average (default: 1)</param>
        /// <returns>Tuple of (mean, standard deviation)</returns>
        public (double mean, double stdDev) GetTheoreticalStatistics(double averagePhotonsPerSecond, double detectionTimeSeconds, int numberOfExposures = 1)
        {
            double lambda = averagePhotonsPerSecond * detectionTimeSeconds;
            
            // The simulation adds offset to raw data, then subtracts it during processing
            // So the theoretical mean should be the photon mean (lambda) without offset
            double mean = lambda;
            
            // For lambda = 0, there's no photon variance, only read noise variance
            double stdDev = 0;
            if (lambda > 0)
            {
                stdDev = Math.Sqrt(lambda); // For Poisson distribution, variance = mean
            }
            
            // Add read noise contribution to variance
            if (readNoise > 0)
            {
                // Total variance = photon variance + read noise variance
                // Read noise variance = readNoise^2
                double totalVariance = lambda + readNoise * readNoise;
                stdDev = Math.Sqrt(totalVariance);
            }
            
            // Account for multiple exposures: 
            // In the simulation, N exposures are SUMMED (each with independent read noise), then divided by N
            if (numberOfExposures > 1)
            {
                // Mean remains the same (sum of N exposures divided by N = average)
                // For summed exposures: variance = N * (photon_variance + read_noise_variance)
                // After dividing by N: variance = (N * (photon_variance + read_noise_variance)) / N² = (photon_variance + read_noise_variance) / N
                // So stdDev = sqrt((photon_variance + read_noise_variance) / N) = original_stdDev / sqrt(N)
                stdDev = stdDev / Math.Sqrt(numberOfExposures);
            }
            
            return (mean, stdDev);
        }

        /// <summary>
        /// Calculates actual statistics from measurement results
        /// </summary>
        /// <param name="measurements">Array of photon detection measurements</param>
        /// <returns>Tuple of (mean, standard deviation, min, max)</returns>
        public (double mean, double stdDev, int min, int max) CalculateStatistics(int[] measurements)
        {
            if (measurements == null || measurements.Length == 0)
                return (0, 0, 0, 0);

            double sum = 0;
            int min = int.MaxValue;
            int max = int.MinValue;

            foreach (int measurement in measurements)
            {
                sum += measurement;
                if (measurement < min) min = measurement;
                if (measurement > max) max = measurement;
            }

            double mean = sum / measurements.Length;
            
            double variance = 0;
            foreach (int measurement in measurements)
            {
                double diff = measurement - mean;
                variance += diff * diff;
            }
            variance /= measurements.Length;
            
            double stdDev = Math.Sqrt(variance);

            return (mean, stdDev, min, max);
        }
    }
} 