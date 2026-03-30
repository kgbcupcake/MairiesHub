#!/bin/bash
set -e

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$PROJECT_ROOT"

echo "=== MairiesHub AppImage Build ==="
echo ""

# Step 1: Publish the application
echo "Step 1: Publishing application as self-contained single binary..."
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
PUBLISH_DIR="$PROJECT_ROOT/bin/Release/net10.0/linux-x64/publish"
echo "✓ Published to $PUBLISH_DIR"
echo ""

# Step 2: Create AppDir structure
echo "Step 2: Creating AppDir structure..."
APPDIR="$PROJECT_ROOT/AppDir"
rm -rf "$APPDIR"
mkdir -p "$APPDIR/usr/bin"
mkdir -p "$APPDIR/usr/share/applications"
mkdir -p "$APPDIR/usr/share/icons/hicolor/256x256/apps"

# Copy the published binary
cp "$PUBLISH_DIR/MairiesHub" "$APPDIR/usr/bin/MairiesHub"
chmod +x "$APPDIR/usr/bin/MairiesHub"
echo "✓ Copied binary to AppDir/usr/bin/"
echo ""

# Step 3: Create .desktop file
echo "Step 3: Creating .desktop file..."
cat > "$APPDIR/usr/share/applications/MairiesHub.desktop" << 'EOF'
[Desktop Entry]
Name=MairiesHub
Exec=MairiesHub
Icon=MairiesHub
Type=Application
Categories=Utility;
EOF
cp "$APPDIR/usr/share/applications/MairiesHub.desktop" "$APPDIR/MairiesHub.desktop"
echo "✓ Created MairiesHub.desktop"
echo ""

# Step 4: Generate placeholder icon (256x256 PNG)
echo "Step 4: Generating placeholder icon..."
python3 << 'PYTHON_SCRIPT'
from PIL import Image, ImageDraw

# Create a 256x256 image with a gradient blue background
img = Image.new('RGB', (256, 256), color=(30, 30, 60))
draw = ImageDraw.Draw(img)

# Draw a simple "MH" in the center
draw.text((85, 100), "MH", fill=(0, 150, 255))

# Save the icon in both locations (as required by appimagetool)
icon_path = '/mnt/storage/RiderProjects/MairiesHub/AppDir/usr/share/icons/hicolor/256x256/apps/MairiesHub.png'
img.save(icon_path)
# Also copy to AppDir root for appimagetool
import shutil
shutil.copy(icon_path, '/mnt/storage/RiderProjects/MairiesHub/AppDir/MairiesHub.png')
PYTHON_SCRIPT

if [ $? -eq 0 ]; then
    echo "✓ Generated placeholder icon"
else
    echo "⚠ PIL/Pillow not available, creating minimal valid PNG..."
    # Fallback: create a minimal 1x1 PNG using hexdump
    PNG_DATA='\x89PNG\r\n\x1a\n\x00\x00\x00\rIHDR\x00\x00\x00\x01\x00\x00\x00\x01\x08\x02\x00\x00\x00\x90wS\xde\x00\x00\x00\x0cIDATx\x9cc\xf8\x0f\x00\x00\x01\x01\x00\x051\x0c\x97\r\x00\x00\x00\x00IEND\xaeB`\x82'
    printf "$PNG_DATA" > "$APPDIR/usr/share/icons/hicolor/256x256/apps/MairiesHub.png"
    printf "$PNG_DATA" > "$APPDIR/MairiesHub.png"
    echo "✓ Created minimal PNG fallback"
fi
echo ""

# Step 5: Create AppRun script
echo "Step 5: Creating AppRun script..."
cat > "$APPDIR/AppRun" << 'EOF'
#!/bin/bash
APPDIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
export LD_LIBRARY_PATH="$APPDIR/usr/lib:$LD_LIBRARY_PATH"
export PATH="$APPDIR/usr/bin:$PATH"
exec "$APPDIR/usr/bin/MairiesHub" "$@"
EOF
chmod +x "$APPDIR/AppRun"
echo "✓ Created AppRun script"
echo ""

# Step 6: Download appimagetool if not present
echo "Step 6: Checking appimagetool..."
APPIMAGETOOL="/tmp/appimagetool"
if [ ! -f "$APPIMAGETOOL" ]; then
    echo "Downloading appimagetool..."
    wget -q -O "$APPIMAGETOOL" "https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage"
    chmod +x "$APPIMAGETOOL"
    echo "✓ Downloaded appimagetool"
else
    echo "✓ appimagetool already present"
fi
echo ""

# Step 7: Build AppImage
echo "Step 7: Building AppImage..."
"$APPIMAGETOOL" "$APPDIR" "$PROJECT_ROOT/MairiesHub-x86_64.AppImage"
chmod +x "$PROJECT_ROOT/MairiesHub-x86_64.AppImage"
echo "✓ Created MairiesHub-x86_64.AppImage"
echo ""

# Step 8: Verify
echo "Step 8: Verifying..."
if [ -f "$PROJECT_ROOT/MairiesHub-x86_64.AppImage" ]; then
    SIZE=$(du -h "$PROJECT_ROOT/MairiesHub-x86_64.AppImage" | cut -f1)
    echo "✓ SUCCESS: AppImage created ($SIZE)"
    echo "Location: $PROJECT_ROOT/MairiesHub-x86_64.AppImage"
    ls -lh "$PROJECT_ROOT/MairiesHub-x86_64.AppImage"
else
    echo "✗ FAILED: AppImage was not created"
    exit 1
fi
