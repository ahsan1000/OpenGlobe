﻿#region License
//
// (C) Copyright 2009 Patrick Cozzi and Deron Ohlarik
//
// Distributed under the Boost Software License, Version 1.0.
// See License.txt or http://www.boost.org/LICENSE_1_0.txt.
//
#endregion

using System.Drawing;
using NUnit.Framework;

namespace MiniGlobe.Renderer
{
    [TestFixture]
    public class Texture2DTests
    {
        [Test]
        public void Texture2DDescription()
        {
            Texture2DDescription description = new Texture2DDescription(512, 256, TextureFormat.RedGreenBlueAlpha8, true);
            Assert.AreEqual(512, description.Width);
            Assert.AreEqual(256, description.Height);
            Assert.AreEqual(TextureFormat.RedGreenBlueAlpha8, description.Format);
            Assert.IsTrue(description.GenerateMipmaps);

            Texture2DDescription description2 = new Texture2DDescription(512, 256, TextureFormat.RedGreenBlueAlpha8, true);
            Assert.AreEqual(description, description2);

            Texture2DDescription description3 = new Texture2DDescription(64, 32, TextureFormat.RedGreenBlueAlpha8, true);
            Assert.AreNotEqual(description, description3);
        }

        [Test]
        public void Texture2DFilter()
        {
            Texture2DFilter filter = new Texture2DFilter(
                TextureMinificationFilter.Linear,
                TextureMagnificationFilter.Nearest,
                TextureWrap.MirroredRepeat,
                TextureWrap.Repeat,
                2);

            Assert.AreEqual(TextureMinificationFilter.Linear, filter.MinificationFilter);
            Assert.AreEqual(TextureMagnificationFilter.Nearest, filter.MagnificationFilter);
            Assert.AreEqual(TextureWrap.MirroredRepeat, filter.WrapS);
            Assert.AreEqual(TextureWrap.Repeat, filter.WrapT);
            Assert.AreEqual(2, filter.MaximumAnisotropic);

            Texture2DFilter filter2 = MiniGlobe.Renderer.Texture2DFilter.LinearClampToEdge;
            Assert.AreNotEqual(filter, filter2);

            Texture2DFilter filter3 = MiniGlobe.Renderer.Texture2DFilter.LinearClampToEdge;
            Assert.AreEqual(filter2, filter3);
        }

        [Test]
        public void Texture2D()
        {
            MiniGlobeWindow window = Device.CreateWindow(1, 1);

            //
            // Create pixel buffer
            //
            BlittableRGBA[] pixels = new BlittableRGBA[]
            {
                new BlittableRGBA(Color.Red), 
                new BlittableRGBA(Color.Green)
            };

            int sizeInBytes = pixels.Length * BlittableRGBA.SizeInBytes;
            WritePixelBuffer writePixelBuffer = Device.CreateWritePixelBuffer(WritePixelBufferHint.StreamDraw, sizeInBytes);
            writePixelBuffer.CopyFromSystemMemory(pixels);

            //
            // Create texture with pixel buffer
            //
            Texture2DDescription description = new Texture2DDescription(2, 1, TextureFormat.RedGreenBlueAlpha8, false);
            Texture2D texture = Device.CreateTexture2D(description);
            texture.CopyFromBuffer(writePixelBuffer, BlittableRGBA.Format, BlittableRGBA.DataType);

            //
            // Read back pixels
            //
            ReadPixelBuffer readPixelBuffer = texture.CopyToBuffer(BlittableRGBA.Format, BlittableRGBA.DataType);
            BlittableRGBA[] readPixels = readPixelBuffer.CopyToSystemMemory<BlittableRGBA>();

            //
            // Verify
            //
            Assert.AreEqual(sizeInBytes, readPixelBuffer.SizeInBytes);
            Assert.AreEqual(pixels[0], readPixels[0]);
            Assert.AreEqual(pixels[1], readPixels[1]);
            Assert.AreEqual(description, texture.Description);

            writePixelBuffer.Dispose();
            texture.Dispose();
            readPixelBuffer.Dispose();
            window.Dispose();
        }

        [Test]
        public void Texture2DSubImage()
        {
            MiniGlobeWindow window = Device.CreateWindow(1, 1);

            //
            // Create pixel buffer
            //
            float[] pixels = new float[]
            {
                1, 2,
                3, 4
            };

            int sizeInBytes = pixels.Length * sizeof(float);
            WritePixelBuffer writePixelBuffer = Device.CreateWritePixelBuffer(WritePixelBufferHint.StreamDraw, sizeInBytes);
            writePixelBuffer.CopyFromSystemMemory(pixels);

            //
            // Create texture with pixel buffer
            //
            Texture2DDescription description = new Texture2DDescription(2, 2, TextureFormat.Red32f, true);
            Texture2D texture = Device.CreateTexture2D(description);
            texture.CopyFromBuffer(writePixelBuffer, ImageFormat.Red, ImageDataType.Float);

            //
            // Read back pixels
            //
            ReadPixelBuffer readPixelBuffer = texture.CopyToBuffer(ImageFormat.Red, ImageDataType.Float);
            float[] readPixels = readPixelBuffer.CopyToSystemMemory<float>();

            //
            // Verify
            //
            Assert.AreEqual(sizeInBytes, readPixelBuffer.SizeInBytes);
            Assert.AreEqual(pixels[0], readPixels[0]);
            Assert.AreEqual(pixels[1], readPixels[1]);
            Assert.AreEqual(pixels[2], readPixels[2]);
            Assert.AreEqual(pixels[3], readPixels[3]);
            Assert.AreEqual(description, texture.Description);

            //
            // Update sub image
            //
            float modifiedPixel = 9;
            writePixelBuffer.CopyFromSystemMemory(new[] { modifiedPixel });
            texture.CopyFromBuffer(writePixelBuffer, 1, 1, 1, 1, ImageFormat.Red, ImageDataType.Float);
            ReadPixelBuffer readPixelBuffer2 = texture.CopyToBuffer(ImageFormat.Red, ImageDataType.Float);
            float[] readPixels2 = readPixelBuffer2.CopyToSystemMemory<float>();

            Assert.AreEqual(sizeInBytes, readPixelBuffer2.SizeInBytes);
            Assert.AreEqual(pixels[0], readPixels2[0]);
            Assert.AreEqual(pixels[1], readPixels2[1]);
            Assert.AreEqual(pixels[2], readPixels2[2]);
            Assert.AreEqual(modifiedPixel, readPixels2[3]);

            writePixelBuffer.Dispose();
            texture.Dispose();
            readPixelBuffer.Dispose();
            readPixelBuffer2.Dispose();
            window.Dispose();
        }

        [Test]
        public void Texture2DAlignment()
        {
            MiniGlobeWindow window = Device.CreateWindow(1, 1);

            //
            // Create pixel buffer
            //
            byte[] pixels = new byte[]
            {
                1, 2, 3, 4, 4, 6,
                7, 8, 9, 10, 11, 12
            };

            int sizeInBytes = pixels.Length * sizeof(byte);
            WritePixelBuffer writePixelBuffer = Device.CreateWritePixelBuffer(WritePixelBufferHint.StreamDraw, sizeInBytes);
            writePixelBuffer.CopyFromSystemMemory(pixels);

            //
            // Create texture with pixel buffer
            //
            Texture2DDescription description = new Texture2DDescription(2, 2, TextureFormat.RedGreenBlue8, false);
            Texture2D texture = Device.CreateTexture2D(description);
            texture.CopyFromBuffer(writePixelBuffer, ImageFormat.RedGreenBlue, ImageDataType.UnsignedByte);

            //
            // Read back pixels
            //
            ReadPixelBuffer readPixelBuffer = texture.CopyToBuffer(ImageFormat.RedGreenBlue, ImageDataType.UnsignedByte);
            byte[] readPixels = readPixelBuffer.CopyToSystemMemory<byte>();

            //
            // Verify
            //
            Assert.AreEqual(sizeInBytes, readPixelBuffer.SizeInBytes);
            for (int i = 0; i < pixels.Length; ++i)
            {
                Assert.AreEqual(pixels[i], readPixels[i]);
            }
            Assert.AreEqual(description, texture.Description);

            writePixelBuffer.Dispose();
            texture.Dispose();
            readPixelBuffer.Dispose();
            window.Dispose();
        }

        [Test]
        public void EnumerateTextureUnits()
        {
            MiniGlobeWindow window = Device.CreateWindow(1, 1);

            int count = 0;
            foreach (TextureUnit unit in window.Context.TextureUnits)
            {
                Assert.IsNull(unit.Texture2D);
                ++count;
            }
            Assert.AreEqual(count, window.Context.TextureUnits.Count);

            window.Dispose();
        }

        [Test]
        public void TextureUnits()
        {
            MiniGlobeWindow window = Device.CreateWindow(1, 1);

            Texture2DDescription description = new Texture2DDescription(1, 1, TextureFormat.RedGreenBlueAlpha8, false);
            Texture2D texture = Device.CreateTexture2D(description);
            texture.Filter = MiniGlobe.Renderer.Texture2DFilter.LinearRepeat;
            Assert.AreEqual(MiniGlobe.Renderer.Texture2DFilter.LinearRepeat, texture.Filter);

            window.Context.TextureUnits[0].Texture2D = texture;
            Assert.AreEqual(texture, window.Context.TextureUnits[0].Texture2D);

            //
            // Attach same texture with different filter
            //
            Texture2DFilter filter2 = new Texture2DFilter(
                TextureMinificationFilter.Nearest,
                TextureMagnificationFilter.Nearest,
                TextureWrap.ClampToEdge,
                TextureWrap.ClampToEdge,
                2);
            texture.Filter = filter2;

            window.Context.TextureUnits[0].Texture2D = texture;
            Assert.AreEqual(texture, window.Context.TextureUnits[0].Texture2D);

            texture.Dispose();
            window.Dispose();
        }
    }
}