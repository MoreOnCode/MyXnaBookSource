// badtoon.fx

// camera and lighting info
float3 CameraPosition : CAMERAPOSITION;
float3 LightDirection = { 0.57f, -0.57f, 0.57f};
float4 LightColor     = { 1, 1, 1, 1};

// color of our teapot
float4 MaterialDiffuseColor  =  { 1.00, 0.80, 0.10, 1.00 };
float4 MaterialSpecularColor  = { 1.00, 0.90, 0.40, 1.00 };
int    MaterialSpecularPower = 2;
float  MaterialSpecularFalloff = 0.33;

// transformations
float4x4 WorldViewProjection : WORLDVIEWPROJECTION;
float4x3 WorldTransform : WORLD;

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

// calculate screen location and colors of this vertex
VertexOutput MainVS(
    float3 ModelPosition : POSITION,
    float3 ModelNormal   : NORMAL )
{
    VertexOutput OutputData = (VertexOutput)0;

    // transform the position and normal from model space to world space
    float3 WorldPosition = 
        mul(float4(ModelPosition, 1), (float4x3)WorldTransform);
    float3 WorldNormal = normalize(mul(ModelNormal, (float3x3)WorldTransform));

    // determine "look at" vector (the direction that the camera is facing)
    float3 LookAt = normalize(WorldPosition - CameraPosition);
    
    OutputData.ScreenPosition = 
        mul( float4( ModelPosition.xyz, 1 ), WorldViewProjection );
    OutputData.DiffuseVertexColor = MaterialDiffuseColor;
    OutputData.SpecularVertexColor =
            CalcSpecularColor (WorldNormal, LightDirection, LookAt); 

	// angle between vertex normal and lookat
    if( max(0 , dot(WorldNormal,-LookAt)) <= 0.30 )
    {
        OutputData.DiffuseVertexColor.a = 0;
    }

    return OutputData;
}


//-----------------------------------
float4 MainPS(VertexOutput InputData): COLOR
{
    // define edge color
    float4 pixelColor = {0,0,0,1};
    float4 diffuseColor = {InputData.DiffuseVertexColor.rgb,1};
    float4 specularColor = {InputData.SpecularVertexColor.rgb,1};
    
    if(InputData.DiffuseVertexColor.a >= 0.33)
    {
        pixelColor.rgb = diffuseColor.rgb;
        if( specularColor.r >= MaterialSpecularColor.r * MaterialSpecularFalloff &&
            specularColor.g >= MaterialSpecularColor.g * MaterialSpecularFalloff &&
            specularColor.b >= MaterialSpecularColor.b * MaterialSpecularFalloff )
        {
    	    pixelColor.rgb = MaterialSpecularColor.rgb;
        }
    }

    return pixelColor;
}


//-----------------------------------
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
        //SHADEMODE = Gouraud; // Not supported on Xbox 360

        // process this model with our shaders
        VertexShader = compile vs_2_0 MainVS();
        PixelShader  = compile ps_2_0 MainPS();
    }
}
