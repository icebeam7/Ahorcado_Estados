using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles.Pages;
using Windows.UI.Popups;

namespace Ahorcado_Estados.Negocios
{
    public class EstadosMSBand
    {
        const string guid = "B9A3B2E5-F917-4B14-9B6B-E14CBCE63A27";
        Guid myTileId;
        string[] codes = new string[] { "001", "002", "006", "008", "011", "014", "015", "016" };
        string path = "ms-appx:///Assets/";
        string ext = ".png";

        public EstadosMSBand()
        {
            myTileId = new Guid(guid);
        }

        private static async Task<BandIcon> LoadIcon(string uri)
        {
            var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            using (var fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                var bitmap = new WriteableBitmap(1, 1);
                bitmap.SetSource(fileStream);
                return bitmap.ToBandIcon();
            }
        }

        private async Task<IBandClient> ConnectToBand()
        {
            var bands = await BandClientManager.Instance.GetBandsAsync();

            if (!bands.Any())
            {
                await ShowDialogAsync("Error", "No band detected");
                return null;
            }

            return await BandClientManager.Instance.ConnectAsync(bands[0]);
        }

        private async Task<bool> CreateTile(IBandClient bandClient)
        {
            try
            {
                int tileCapacity = await bandClient.TileManager.GetRemainingTileCapacityAsync();

                if (tileCapacity > 0)
                {
                    BandTile tile = new BandTile(myTileId)
                    {
                        Name = "Ahorcado",
                        TileIcon = await LoadIcon(String.Format("{0}Logo46x46{1}", path, ext)),
                        SmallIcon = await LoadIcon(String.Format("{0}Logo24x24{1}", path, ext))
                    };

                    foreach (string code in codes)
                        tile.AdditionalIcons.Add(await LoadIcon(String.Format("{0}{1}{2}", path, code, ext)));

                    if (await bandClient.TileManager.AddTileAsync(tile))
                    {
                        CreateLayout(tile);
                        await bandClient.NotificationManager.SendMessageAsync(myTileId, "Ahorcado", "Tile was just created!",
                            DateTimeOffset.Now, MessageFlags.None);

                        return true;
                    }
                    else
                    {
                        await ShowDialogAsync("Error", "Tile not created");
                        return false;
                    }
                }
                else
                {
                    await ShowDialogAsync("Error", "No space on Band for tile");
                    return false;
                }
            }
            catch (BandException ex)
            {
                ShowDialogAsync("Error", "Error creating tile. Exception found: " + ex.Message);
                return false;
            }
        }

        private async Task SendData(Estados estado)
        {
            try
            {
                IBandClient bandClient = await ConnectToBand();

                if (bandClient != null)
                {
                    using (bandClient)
                    {
                        if (await CreateTile(bandClient))
                        {
                            await bandClient.NotificationManager.SendMessageAsync(myTileId, "Victory", "You just discovered " + estado.Nombre,
                                DateTimeOffset.Now, MessageFlags.ShowDialog);
                        }
                    }
                }
            }
            catch (BandException ex)
            {
                ShowDialogAsync("Error", "Error sending data. Exception found: " + ex.Message);
            }
        }

        private void CreateLayout(BandTile tile)
        {
            ScrollFlowPanel panel = new ScrollFlowPanel()
            {
                Rect = new PageRect(0, 0, 245, 102),
                Orientation = FlowPanelOrientation.Vertical
            };

            Icon icon = new Icon()
            {
                ElementId = 3,
                Rect = new PageRect(0, 0, 245, 102),
                Margins = new Margins(15, 0, 15, 0),
                VerticalAlignment = VerticalAlignment.Top,
            };

            var nombre = new WrappedTextBlock
            {
                ElementId = 1,
                Rect = new PageRect(0, 0, 245, 102),
                Margins = new Margins(15, 0, 15, 0),
                Color = new BandColor(0xFF, 0xFF, 0xFF),
                Font = WrappedTextBlockFont.Small,
                VerticalAlignment = VerticalAlignment.Top
            };

            var motto = new WrappedTextBlock
            {
                ElementId = 2,
                Rect = new PageRect(0, 0, 245, 102),
                Margins = new Margins(15, 0, 15, 0),
                Color = new BandColor(0xFF, 0xFF, 0xFF),
                Font = WrappedTextBlockFont.Small,
                VerticalAlignment = VerticalAlignment.Top
            };

            panel.Elements.Add(icon); 
            panel.Elements.Add(nombre);
            panel.Elements.Add(motto);

            PageLayout layout = new PageLayout(panel);
            tile.PageLayouts.Add(layout);
        }

        private async Task SetContent(Estados estado)
        {
            try
            {
                IBandClient bandClient = await ConnectToBand();

                if (bandClient != null)
                {
                    using (bandClient)
                    {
                        Guid messagesPageGuid = Guid.NewGuid();

                        PageData pageContent = new PageData(messagesPageGuid, 0,
                            new WrappedTextBlockData(1, estado.Nombre),
                            new WrappedTextBlockData(2, estado.Motto),
                            new IconData(3, GetId(estado.Codigo)));

                        await bandClient.TileManager.SetPagesAsync(myTileId, pageContent);
                    }
                }
            }
            catch (BandException ex)
            {
                ShowDialogAsync("Error", "Error setting content. Exception found: " + ex.Message);                
            }
        }

        private ushort GetId(string codigo)
        {
            ushort value = 0; 
            int index = codes.Select((s, i) => new { i, s }).Where(t => t.s == codigo).Select(t => t.i).FirstOrDefault();
            
            if (index > -1)
                value = (ushort)(index + 2);
            
            return value;
        }

        public async Task Publish(Estados estado)
        {
            await SendData(estado);
            await SetContent(estado);
        }

        private async Task ShowDialogAsync(string title, string message)
        {
            MessageDialog box = new MessageDialog(title, message);
            await box.ShowAsync();
        }
    }
}
