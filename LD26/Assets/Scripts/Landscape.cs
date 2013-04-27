using UnityEngine;
using System.Collections;

public class Landscape : MonoBehaviour
{
	public float islandRadius;
	public float verticalStretch;
	
	// Internal vars
	private Mesh emptyMesh;
	private Texture2D texture;
	private Vector3[] vertices;
	private Vector2[] uv;
	private Vector3[] normals;
	private float[] heightfield;
	private int[] triangles;
	private int width 	= 128;
	private int height 	= 128;
	private float mapsize = 100.0f;
	
	private Perlin 			perlin;
	//private FractalNoise 	fractal;
	
	// Use this for initialization
	void Start ()
	{
		perlin = new Perlin();
		
		// Create the game object containing the renderer
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshCollider>();
		
		emptyMesh = new Mesh();	// empty mesh
		emptyMesh.vertices = new Vector3[3];
		emptyMesh.triangles = new int[3];
		emptyMesh.vertices[0] = Vector3.zero;
		emptyMesh.vertices[1] = Vector3.forward;
		emptyMesh.vertices[2] = Vector3.left;
		emptyMesh.triangles[0] = 0;
		emptyMesh.triangles[1] = 1;
		emptyMesh.triangles[2] = 2;
		
		// Allocate all storage for mesh, since size will not vary on regeneration - only content				
		vertices 	= new Vector3[height * width];
		uv 			= new Vector2[height * width];
		normals 	= new Vector3[height * width];
		heightfield = new float[(height+2) * (width+2)];
		triangles 	= new int[(height - 1) * (width - 1) * 6];
		texture 	= new Texture2D( width, height, TextureFormat.RGB24, true, true);
		texture.filterMode = FilterMode.Trilinear;
	
		renderer.material.color = Color.white;
		renderer.material.mainTexture = texture;
		renderer.material.mainTexture.wrapMode = TextureWrapMode.Clamp;
		
		GenerateMesh();
	}
	
	float GetFractal( float x, float y )
	{
		float localScale = 1.0f;
		float distFromOrigin = Mathf.Sqrt(x*x + y*y);
		if (distFromOrigin > islandRadius)
			return 0.0f;
		
		localScale = (distFromOrigin/islandRadius);
		localScale = (1.0f - localScale)*(1.0f - localScale);	// Inverted square curve to get steeper shores
		
		/*
		float jagginess = 0.75f * (1.0f + fractal.HybridMultifractal( (x+jaggiScale)*scale*oneOverJagginessScale, y*scale*oneOverJagginessScale, offset ));
		
		float pixelHeight = fractal.HybridMultifractal( x*scale, y*scale, offset ) * jagginess;
		
		pixelHeight += fractal.HybridMultifractal( x*scale*oneOverMegaScale, y*scale*oneOverMegaScale, offset ) * megaScale;
		*/
		
		float pixelHeight = 1.0f + perlin.Noise( x, y );
		
		pixelHeight *= verticalStretch * localScale;
		
		return pixelHeight;
	}
	
	void GenerateMesh()
	{
		Debug.Log("Starting mesh generation...");
		
		Random.seed = (int)(transform.position.x + 3.0f * transform.position.z);
		
		// Retrieve a mesh instance
		Mesh mesh = GetComponent<MeshFilter>().mesh;
			
		// Build vertices and UVs
		Vector2 uvScale 	= new Vector2(1.0f / (float)(width+1), 1.0f / (float)(height+1));
		float oneOverXCells = 1.0f / (float)(width-1);
		float oneOverZCells = 1.0f / (float)(height-1);
		
		// Pass one - fill in corner heights
		Debug.Log(" Pass 1 - fill in fractal points");
		for (int y=0; y<=height; y++)
		{
			for (int x=0; x<=width; x++)
			{
				float pixelHeight = GetFractal( transform.position.x+(x*mapsize*oneOverXCells),
				                                transform.position.z+(y*mapsize*oneOverZCells) );
				heightfield[y*(width+2) + x] = pixelHeight;
			}
		}
		
		// Pass two - generate geometry
		Debug.Log(" Pass 2 - generate geometry");
		float checkDimness = 0.8f;
		for (int y=0; y<height; y++)
		{
			for (int x=0; x<width; x++)
			{
				Color col = Color.white;
			
				float p1 = heightfield[y*(width+2)+x];
				float p2 = heightfield[y*(width+2)+x+1];
				float p3 = heightfield[(y+1)*(width+2)+x];
				float p4 = heightfield[(y+1)*(width+2)+x+1];
	
				Vector3 vertex;
				vertex.x = mapsize*x/(width-2.0f);
				vertex.z = mapsize*y/(height-2.0f);
				vertex.y = 0.0f; //heightfield[y*(width+2)+x];
				Debug.Log("V"+x+"."+y+" "+vertex.x+", "+vertex.z);
				
				vertices[y*width + x] = vertex;
				Vector2 temp_uv = new Vector2((float)x, (float)y);
				uv[y*width + x] = Vector2.Scale(temp_uv, uvScale);
				
				normals[y*width + x] = Vector3.up;
				
				texture.SetPixel(x, y, col);		
			}
		}
		
		// Pass 4 - build index buffer
		//Debug.Log(" Pass 4 - build index buffer");
		
		texture.Apply();
		
		// Assign them to the mesh
		mesh.vertices = vertices;
		mesh.uv = uv;
	
		// Build triangle indices: 3 indices into vertex array for each triangle
		int index = 0;
		for (int y=0; y<height-1; y++)
		{
			for (int x=0; x<width-1; x++)
			{
				// For each grid cell output two triangles
				float p1 = heightfield[y*(width+2)+x];
				float p2 = heightfield[y*(width+2)+x+1];
				float p3 = heightfield[(y+1)*(width+2)+x];
				float p4 = heightfield[(y+1)*(width+2)+x+1];
				
				if (p1+p2+p3+p4 > 0.001f)	// Skip generating triangles at water level
				{
					if (p1 < p4 - 0.01f || p4 < p1 - 0.01f)
					{
						triangles[index++] = (y     * width) + x;
						triangles[index++] = ((y+1) * width) + x;
						triangles[index++] = ((y+1) * width) + x + 1;
			
						triangles[index++] = ((y+1) * width) + x + 1;
						triangles[index++] = (y     * width) + x + 1;			
						triangles[index++] = (y     * width) + x;
					}
					else
					{
						triangles[index++] = (y     * width) + x;
						triangles[index++] = ((y+1) * width) + x;
						triangles[index++] = (y     * width) + x + 1;
			
						triangles[index++] = ((y+1) * width) + x;
						triangles[index++] = ((y+1) * width) + x + 1;
						triangles[index++] = (y     * width) + x + 1;
					}
				}
			}
		}
		// And assign them to the mesh
		mesh.triangles = triangles;
			
		// Auto-calculate vertex normals from the mesh
		//mesh.normals = normals;
		mesh.RecalculateNormals();
		
		MeshCollider collider = transform.GetComponent<MeshCollider>();
		collider.sharedMesh = emptyMesh;	// Flush cached physics-friendly version of old mesh
		collider.sharedMesh = mesh;
		
		Debug.Log("Finished mesh generation.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
