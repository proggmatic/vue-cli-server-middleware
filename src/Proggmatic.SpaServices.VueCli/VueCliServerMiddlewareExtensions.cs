using System;

using Microsoft.AspNetCore.SpaServices;


namespace Proggmatic.SpaServices.VueCli
{
    /// <summary>
    /// Extension methods for enabling Vue development server middleware support.
    /// </summary>
    public static class VueCliServerMiddlewareExtensions
    {
        /// <summary>
        /// Handles requests by passing them through to an instance of the vue-cli-service server.
        /// This means you can always serve up-to-date CLI-built resources without having
        /// to run the vue-cli-service server manually.
        ///
        /// This feature should only be used in development. For production deployments, be
        /// sure not to enable the vue-cli-service server.
        /// </summary>
        /// <param name="spaBuilder">The <see cref="ISpaBuilder"/>.</param>
        /// <param name="npmScript">The name of the script in your package.json file that launches the vue-cli-service server.</param>
        public static void UseVueCliServer(
            this ISpaBuilder spaBuilder,
            string npmScript = "serve")
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            var spaOptions = spaBuilder.Options;

            if (string.IsNullOrEmpty(spaOptions.SourcePath))
                spaOptions.SourcePath = "ClientApp";

            if (string.IsNullOrEmpty(spaOptions.PackageManagerCommand))
                spaOptions.PackageManagerCommand = "npm";

            VueCliServerMiddleware.Attach(spaBuilder, npmScript);
        }
    }
}