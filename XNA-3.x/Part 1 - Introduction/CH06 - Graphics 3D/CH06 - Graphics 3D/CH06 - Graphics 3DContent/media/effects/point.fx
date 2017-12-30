// point.fx

// camera and lighting info
float3 CameraPosition : CAMERAPOSITION;

// color of our teapot
float4 MaterialDiffuseColor  = { 1.00, 0.80, 0.10, 1.00 };

// transformations
float4x4 WorldViewProjection : WORLDVIEWPROJECTION;
float4x3 WorldTransform : WORLD;

// vertex shader output is stored in this struct, then passed to pixel shader
struct VertexOutput
{
    float4 ScreenPosition      : POSITION;
    float4 DiffuseVertexColor  : COLOR0;
};

// calculate screen location, base color, and texture coordinate of this vertex
VertexOutput MainVS (
    float3 ModelPosition : POSITION,
    float3 ModelNormal   : NORMAL )
{
    VertexOutput OutData = (VertexOutput)0;
    
    // convert current vertex from model space to screen space 
    // (the primary job of any vertex shader)
    OutData.ScreenPosition = mul(float4(ModelPosition,1), WorldViewProjection);
    
    // calculate vertex color, based on our one directional lightsource
    OutData.DiffuseVertexColor.rgba = MaterialDiffuseColor;

    return OutData;
}

// calculate the final color of our model's current pixel
float4 MainPS( VertexOutput InputData ) : COLOR
{
    return InputData.DiffuseVertexColor;
}

// every shader needs at least one technique
technique TheOnlyTechnique
{
    // your techniques may have multiple passes, 
    // this particular example just has one
    pass SinglePass
    {
        // don't hide back faces
        CULLMODE = None;
        
        // render this model as a solid (opaque) body
        FILLMODE  = Point;
        //SHADEMODE = Flat; // Not supported on Xbox 360
        POINTSIZE = 2.0f;

          // process this model with our shaders
        VertexShader = compile vs_2_0 MainVS();
        PixelShader  = compile ps_2_0 MainPS();
    }
}