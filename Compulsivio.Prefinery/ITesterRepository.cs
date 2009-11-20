using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compulsivio.Prefinery
{
    /// <summary>
    /// A repository for managing related <see cref="T:Compulsivio.Prefinery.ITester"/> objects.
    /// </summary>
    public interface ITesterRepository
    {
        /// <summary>
        /// Finds a tester in the repository with the specified identification number.
        /// </summary>
        /// <param name="id">The identification number of the tester.</param>
        /// <returns>An <see cref="T:Compulsivio.Prefinery.ITester"/> representing the tester.</returns>
        ITester GetTester(int id);

        /// <summary>
        /// Finds a tester in the repository with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address of the tester.</param>
        /// <returns>An <see cref="T:Compulsivio.Prefinery.ITester"/> representing the tester.</returns>
        ITester GetTester(string email);

        /// <summary>
        /// Gets a list of all testers managed by the repository.
        /// </summary>
        /// <returns>An enumerable list of <see cref="T:Compulsivio.Prefinery.ITester"/> objects.</returns>
        IEnumerable<ITester> GetTesters();

        /// <summary>
        /// Adds a tester to the repository.
        /// </summary>
        /// <param name="tester">A <see cref="T:Compulsivio.Prefinery.ITester"/> for the repository to manage.</param>
        void AddTester(ITester tester);

        /// <summary>
        /// Submit changes to a tester to Prefinery.
        /// </summary>
        /// <param name="tester">The <see cref="T:Compulsivio.Prefinery.ITester"/> which has been modified.</param>
        void UpdateTester(ITester tester);

        /// <summary>
        /// Remove a tester from the repository and Prefinery.
        /// </summary>
        /// <param name="tester">The <see cref="T:Compulsivio.Prefinery.ITester"/> to delete.</param>
        void DeleteTester(ITester tester);
    }
}
