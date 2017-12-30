// flat.fx

// camera and lighting info
float3 CameraPosition : CAMERAPOSITION;
float3 LightDirection = { 0.57f, -0.57f, 0.57f};
float4 LightColor     = { 1, 1, 1, 1};

// color of our teapot
float4 MaterialAmbientColor  =  { 0.20, 0.00, 0.00, 1.00 };
float4 MaterialDiffuseColor  =  { 1.00, 0.80, 0.10, 1.00 };
float4 MaterialSpecularColor  = { 1.00, 0.90, 0.80, 1.00 };
int    MaterialSpecularPower = 2;

// transformations
float4x4 WorldViewProjection : WORLDVIEWPROJECTION;
float4x3 WorldTransform : WORLD;

// calculate diffuse component of this vertex's color 
float4 CalcDiffuseColor(float3 ViewNormal, float3 LightDirection)
{
    return 
        (MaterialAmbientColor + MaterialDiffuseColor) * 
        LightColor * 
        saturate(dot(ViewNormal, -LightDirection));
}

// calculate specular component of this vertex's color 
float4 CalcSpecularColor(
    float3 ViewNormal, float3 LightDirection, float3 LookAt)
{
    float3 ReflectedLight = normalize( reflect(LightDirection,ViewNormal) );
    return 
        MaterialSpecularColor * 
        LightColor * 
        pow( saturate(dot(ReflectedLight, -LookAt) ), 
            MaterialSpecularPower);
}

// vertex shader output is stored in this struct, then passed to pixel shader
struct VertexOutput
{
    float4 ScreenPosition      : POSITION;
    float4 DiffuseVertexColor  : COLOR0;
    float4 SpecularVertexColor : COLOR1;
};

// calculate screen location, base color, and texture coordinate of this vertex
VertexOutput MainVS (
    float3 ModelPosition : POSITION,
    float3 ModelNormal   : NORMAL )
{
    VertexOutput OutData = (VertexOutput)0;
    
    // transform the position and normal from model space to world space
    float3 WorldPosition = 
        mul(float4(ModelPosition, 1), (float4x3)WorldTransform);
    float3 WorldNormal = normalize(mul(ModelNormal, (float3x3)WorldTransform));

    // determine "look at" vector (the direction that the camera is facing)
    float3 LookAt = normalize(WorldPosition - CameraPosition);
    
    // convert current vertex from model space to screen space 
    // (the primary job of any vertex shader)
    OutData.ScreenPosition = mul(float4(ModelPosition,1), WorldViewProjection);
    
    // calculate vertex color, based on our one directional lightsource
    OutData.DiffuseVertexColor.rgba =
        CalcDiffuseColor (WorldNormal, LightDirection);
    OutData.SpecularVertexColor.rgba =
        CalcSpecularColor (WorldNormal, LightDirection, LookAt); 

    return OutData;
}

// calculate the final color of our model's current pixel
float4 MainPS( VertexOutput InputData ) : COLOR
{
    return InputData.DiffuseVertexColor + InputData.SpecularVertexColor;
}

// every shader needs at least one technique
technique TheOnlyTechnique
{
    // your techniques may have multiple passes, 
    // this particular example just has one
    pass SinglePass
    {
        // don't hide back faces, the teapot has gaps that will show through
        CULLMODE = None;
        
        // render this model as a solid (opaque) body
        FILLMODE  = Solid;
        //SHADEMODE = Flat; // Not supported on Xbox 360

          // process this model with our shaders
        VertexShader = compile vs_2_0 MainVS();
        PixelShader  = compile ps_2_0 MainPS();
    }
}