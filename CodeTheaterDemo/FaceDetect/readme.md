# Using a QueueTrigger Function for Face detection and storage

In this sample, we use an Azure Function to take an input message from an Azure Queue and then submit the image URL contained within to the Microsoft Cognitive Services Face API. It then gets the results and stores them in a DocumentDB collection.

# Custom Application Settings
For this sample to run in Azure, you need to set the following settings in the portal under 'Platform Features->Application settings'

'FACE_API_KEY' - this is the API key that can be obtained from either the Microsoft Cognitive Services site(free tier) or your Azure account(paid tier).
'DOCDB_ENDPOINT_STRING' - this is found in the settings pane of your DocumentDB deployment in Azure.
'DOCDB_AUTH_KEY' - this is found in the settings pane of your DocumentDB deployment in Azure.