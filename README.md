# Advanced Artificial Intelligence for Games - LLM Unity

## Setup

### Download LLM models
1. Open the `SageLLMAndRAG` scene under `Assets/Scenes/`
2. Click on the `LLM` GameObject in the hierarchy
   1. In the inspector, under `Model Settings` remove the existing models by clicking the trash icon (if they are present)
   2. Click the `Download model` button
   3. Hover over `Medium models` and click on `Llama 3.1 8B`
   4. After the model is downloaded (can take up to 5-10 minutes), change the following settings:
      1. Set `Chat template` to `chatml (most widely used)`
      2. Set `Context size` to `512`
3. Click on the `LLMRAG` GameObject in the hierarchy
   1. In the inspector, under `Model Settings`, click the `Download model` button
   2. Hover over `RAG models` and click on `BGE large en v1.5`
   3. After the model is downloaded (can take up to 2 minutes), change the following settings:
       1. Set `Chat template` to `chatml (most widely used)`
       2. Set `Context size` to `512`

## Run the project
1. Open the `Town` scene under `Assets/Scenes/`
2. Click the play button in the Unity editor
