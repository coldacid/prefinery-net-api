using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    /// <summary>
    /// A repository for managing related <see cref="T:Compulsivio.Prefinery.IBeta"/> objects.
    /// </summary>
    public interface IBetaRepository
    {
        /// <summary>
        /// Finds a beta with a specified identification number.
        /// </summary>
        /// <param name="id">The identification number of the beta to return.</param>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.IBeta"/> representing the beta.</returns>
        IBeta GetBeta(int id);

        /// <summary>
        /// Finds a beta with a specified name.
        /// </summary>
        /// <param name="name">The name of the beta to return.</param>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.IBeta"/> representing the beta.</returns>
        /// <remarks>
        /// This is the name given to the beta in the application configuration file, or set when
        /// creating a beta in code. This is <strong>not</strong> the name by which the beta is
        /// known to Prefinery.
        /// </remarks>
        IBeta GetBeta(string name);

        /// <summary>
        /// Gets all betas managed by the repository.
        /// </summary>
        /// <returns>An enumerable list of <see cref="T:Compulsivio.Prefinery.IBeta"/> objects.</returns>
        IEnumerable<IBeta> GetBetas();

        // NOT IN PREFINERY API YET

        /// <summary>
        /// Adds a beta to the repository for managing.
        /// </summary>
        /// <param name="beta">A <see cref="T:Compulsivio.Prefinery.IBeta"/> for the repository to manage.</param>
        void AddBeta(IBeta beta);

        ////void UpdateBeta(IBeta beta);

        ////void DeleteBeta(IBeta beta);
    }
}
