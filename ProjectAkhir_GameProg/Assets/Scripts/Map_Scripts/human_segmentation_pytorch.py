import cv2
import torch
import torchvision
import numpy as np
from torchvision import transforms

# Load DeepLabV3 model (lightweight backbone)
model = torchvision.models.segmentation.deeplabv3_mobilenet_v3_large(pretrained=True)
model.eval()

# Use Apple GPU if available
device = torch.device("mps") if torch.backends.mps.is_available() else torch.device("cpu")
model.to(device)

# Transformation
preprocess = transforms.Compose([
    transforms.ToTensor(),
    transforms.Normalize(mean=[0.485, 0.456, 0.406],
                         std=[0.229, 0.224, 0.225])
])

# Video capture (0 = webcam)
cap = cv2.VideoCapture(0)

while True:
    ret, frame = cap.read()
    if not ret:
        break

    # Resize for speed
    small_frame = cv2.resize(frame, (320, 320))

    # Preprocess
    input_tensor = preprocess(small_frame).unsqueeze(0).to(device)

    # Predict mask
    with torch.no_grad():
        output = model(input_tensor)['out'][0]
        mask = output.argmax(0).cpu().numpy()

    # Human class in COCO = 15
    human_mask = (mask == 15).astype(np.uint8)
    human_mask = cv2.resize(human_mask, (frame.shape[1], frame.shape[0]))

    # Apply mask
    blurred = cv2.GaussianBlur(frame, (55, 55), 0)
    result = np.where(human_mask[:, :, None], frame, blurred)

    cv2.imshow("Human Segmentation", result)

    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()
