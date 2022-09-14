using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Extensions.Configuration;

namespace AzureAILabb2WFA
{
    public partial class Form1 : Form
    {
        static CustomVisionPredictionClient prediction_client;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView2.View = View.Details;

            listView1.Columns.Add("Vehicles", 150);
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            listView2.Columns.Add("Predictions", 150);
            listView2.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            Populate();
        }

        
        private void Populate()
        {
            ImageList img = new ImageList();
            img.ImageSize = new Size(200, 200);

            String[] paths = { };
            paths = Directory.GetFiles("C:\\test-images");

            try
            {
                foreach (String path in paths)
                {
                    img.Images.Add(Image.FromFile(path));
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }



            listView1.SmallImageList = img;
            listView1.Items.Add("1", 0);
            listView1.Items.Add("2", 1);
            listView1.Items.Add("3", 2);
            listView1.Items.Add("4", 3);



        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MakePredictionRequest();
        }
        private void MakePredictionRequest()
        {
            try
            {

                string prediction_endpoint = "https://cusvislabb2-prediction.cognitiveservices.azure.com/";
                string prediction_key = "e2828558f85348d4b00d923bf78b9030";
                Guid project_id = Guid.Parse("8193f5b0-78d1-441d-b157-ff1a547cf789");
                string model_name = "vehicle-classifier";

                // Authenticate a client for the prediction API
                prediction_client = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(prediction_key))
                {
                    Endpoint = prediction_endpoint
                };

                // Classify test images
                String[] images = Directory.GetFiles("C:\\test-images");
                int count = 1;

                foreach (var image in images)
                {
                    MemoryStream image_data = new MemoryStream(File.ReadAllBytes(image));
                    var result = prediction_client.ClassifyImage(project_id, model_name, image_data);

                    // Loop over each label prediction and print any with probability > 50%
                    foreach (var prediction in result.Predictions)
                    {
                        if (prediction.Probability > 0.5)
                        {
                            
                            listView2.Items.Add($"vehicle: {count} is a {prediction.TagName} ({prediction.Probability:P1}");
                        }
                        
                    }
                    count++;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}