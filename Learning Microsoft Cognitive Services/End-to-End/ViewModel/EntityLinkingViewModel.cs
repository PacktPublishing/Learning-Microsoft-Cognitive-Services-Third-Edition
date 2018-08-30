using End_to_End.Interface;
using System.Windows.Input;
using End_to_End.Model;
using System.Windows;
using System.Text;
using Microsoft.ProjectOxford.EntityLinking.Contract;

namespace End_to_End.ViewModel
{
    public class EntityLinkingViewModel : ObservableObject
    {
        private EntityLinking _entityLinking;

        private string _selection;
        public string Selection
        {
            get { return _selection; }
            set
            {
                _selection = value;
                RaisePropertyChangedEvent("Selection");
            }
        }

        private int _offset;
        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                RaisePropertyChangedEvent("Offset");                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
            }
        }
        
        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
                RaisePropertyChangedEvent("InputText");
            }
        }

        private string _resultText;
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = value;
                RaisePropertyChangedEvent("ResultText");
            }
        }

        public ICommand LinkEntitiesCommand { get; private set; }

        /// <summary>
        /// EntityLinkingViewModel
        /// Creates a new <see cref="EntityLinking"/> object and the ICommand object
        /// Subscribes to the EntityLinkingError event
        /// </summary>
        public EntityLinkingViewModel()
        {
            LinkEntitiesCommand = new DelegateCommand(LinkEntities, CanLinkEntities);

            _entityLinking = new EntityLinking("API_KEY_HERE");
            _entityLinking.EntityLinkingError += OnEntityLinkingError;
        }

        /// <summary>
        /// Function to determine if we can link entities or not
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if input text has been provided, false otherwise</returns>
        private bool CanLinkEntities(object obj)
        {
            return !string.IsNullOrEmpty(InputText);
        }

        /// <summary>
        /// Function to execute entity linking
        /// If a successful call has been made, it will parse the results and display it in the UI
        /// </summary>
        /// <param name="obj"></param>
        private async void LinkEntities(object obj)
        {
            EntityLink[] linkedEntities = await _entityLinking.LinkEntities(InputText, Selection, Offset);

            if(linkedEntities == null || linkedEntities.Length == 0)
            {
                ResultText = "No linked entities found";
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Entities found: {0}\n\n", linkedEntities.Length);

            foreach (EntityLink entity in linkedEntities)
            {
                sb.AppendFormat("Entity '{0}'\n\tScore {1}\n\tWikipedia ID '{2}'\n\tMatches in text: {3}\n\n", 
                    entity.Name, entity.Score, entity.WikipediaID, entity.Matches.Count);

                foreach (var match in entity.Matches)
                {
                    sb.AppendFormat("Text match: '{0}'\n", match.Text);

                    sb.Append("Found at position: ");
                    foreach (var entry in match.Entries)
                    {
                        sb.AppendFormat("{0}\t", entry.Offset);
                    }

                    sb.Append("\n\n");
                }
            }

            ResultText = sb.ToString();
        }

        /// <summary>
        /// Function to handle errors in the entity linking process.
        /// Errors will be displayed in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntityLinkingError(object sender, EntityLinkingErrorEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {
                ResultText = $"Failed to link entities, with error message: {e.ErrorMessage}";
            });
        }
    }
}