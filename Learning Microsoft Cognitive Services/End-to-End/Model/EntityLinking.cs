using Microsoft.ProjectOxford.EntityLinking;
using Microsoft.ProjectOxford.EntityLinking.Contract;
using System;
using System.Threading.Tasks;

namespace End_to_End.Model
{
    public class EntityLinking
    {
        public event EventHandler<EntityLinkingErrorEventArgs> EntityLinkingError;

        private EntityLinkingServiceClient _entityLinkingServiceClient;

        /// <summary>
        /// EntityLinking constructor. Creates a new <see cref="EntityLinkingServiceClient"/> object
        /// </summary>
        /// <param name="apiKey">Entity Linking API key</param>
        public EntityLinking(string apiKey)
        {
            _entityLinkingServiceClient = new EntityLinkingServiceClient(apiKey, "ROOT_URI");
        }

        /// <summary>
        /// Function to link entities
        /// </summary>
        /// <param name="inputText">Input text to link entities from</param>
        /// <param name="selection">Given entity to link from, defaults to empty string</param>
        /// <param name="offset">If an entity has been given, this offset specifies the first place its found in the text. Defaults to 0</param>
        /// <returns>Array containing <see cref="EntityLink"/> objects</returns>
        public async Task<EntityLink[]> LinkEntities(string inputText, string selection = "", int offset = 0)
        {
            try
            {
                EntityLink[] linkingResponse = await _entityLinkingServiceClient.LinkAsync(inputText, selection, offset);

                return linkingResponse;
            }
            catch(Exception ex)
            {
                RaiseOnEntityLinkingError(new EntityLinkingErrorEventArgs(ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Helper function to raise EntityLinkingError events
        /// </summary>
        /// <param name="args"></param>
        private void RaiseOnEntityLinkingError(EntityLinkingErrorEventArgs args)
        {
            EntityLinkingError?.Invoke(this, args);
        }
    }

    /// <summary>
    /// EventArgs class containing error messages for entity linking errors
    /// </summary>
    public class EntityLinkingErrorEventArgs
    {
        public string ErrorMessage { get; private set; }

        public EntityLinkingErrorEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}