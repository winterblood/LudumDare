using UnityEngine;
using System.Collections;

public class Landscape : MonoBehaviour
{
	public float scale 				= 0.2f;
	public float verticalStretch 	= 25.0f;
	public float islandRadius 		= 60.0f;
	public int treeCount	 		= 10;
	public float uvFudge			= 0.975f;	// Awful hack to line texture alterations up with player, couldn't find the root error
	
	public GameObject treePrefab;
	
	// Internal vars
	private Mesh emptyMesh;
	private Texture2D texture;
	private Vector3[] vertices;
	private Vector2[] uv;
	//private Vector3[] normals;
	private float[] heightfield;
	private int[] triangles;
	private int width 	= 128;
	private int height 	= 128;
	private int texSize	= 256;
	private float mapsize = 200.0f;
	private int framecount = 0;
	private GameObject megaTree;
		
	private Perlin 			perlin;
	
	// Use this for initialization
	void Start ()
	{
		Application.targetFrameRate = 30;
	
		perlin = new Perlin();
		mapsize = islandRadius * 2.0f;
		
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
		//normals 	= new Vector3[height * width];
		heightfield = new float[(height+2) * (width+2)];
		triangles 	= new int[(height - 1) * (width - 1) * 6];
		texture 	= new Texture2D( texSize, texSize, TextureFormat.RGB24, false, true);
		texture.filterMode = FilterMode.Bilinear;
	
		renderer.castShadows	= false;
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
		localScale = 1.0f-(localScale*localScale);	// square curve to get steeper shores
		
		/*
		float jagginess = 0.75f * (1.0f + fractal.HybridMultifractal( (x+jaggiScale)*scale*oneOverJagginessScale, y*scale*oneOverJagginessScale, offset ));
		
		float pixelHeight = fractal.HybridMultifractal( x*scale, y*scale, offset ) * jagginess;
		
		pixelHeight += fractal.HybridMultifractal( x*scale*oneOverMegaScale, y*scale*oneOverMegaScale, offset ) * megaScale;
		*/
		
		float pixelHeight = 1.0f + perlin.Noise( x*scale, y*scale );
		
		pixelHeight *= verticalStretch * localScale;
		
		return pixelHeight;
	}
	
	public float GetTerrainHeight( float x, float z )
	{
		// Map world coords to cell
		float lx = (x + mapsize * 0.5f)*(width-2.0f)/mapsize;
		float ly = (z + mapsize * 0.5f)*(height-2.0f)/mapsize;
		int cx = (int)lx;
		int cy = (int)ly;
		lx -= cx;		// Calc fractions across cell
		ly -= cy;
		
		float p1 = heightfield[cy*(width+2)+cx];
		float p2 = heightfield[cy*(width+2)+cx+1];
		float p3 = heightfield[(cy+1)*(width+2)+cx];
		float p4 = heightfield[(cy+1)*(width+2)+cx+1];
		
		float tx = p1*(1.0f-lx) + p2*lx;	// Top edge
		float bx = p3*(1.0f-lx) + p4*lx;	// Bottom edge
		
		return tx*(1.0f-ly) + bx*ly;
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
		
		// Pass one - fill in corner heights, and remember highest point!
		Debug.Log(" Pass 1 - fill in fractal points");
		
		int highestx = 0;
		int highesty = 0;
		float highest = -999.0f;
		for (int y=0; y<=height; y++)
		{
			for (int x=0; x<=width; x++)
			{
				float pixelHeight = GetFractal( transform.position.x-mapsize*0.5f+(x*mapsize*oneOverXCells),
				                                transform.position.z-mapsize*0.5f+(y*mapsize*oneOverZCells) );
				heightfield[y*(width+2) + x] = pixelHeight;
				
				if (pixelHeight > highest)
				{
					highest = pixelHeight;
					highestx = x;
					highesty = y;
				}
			}
		}
		
		// Pass two - generate geometry
		Debug.Log(" Pass 2 - generate geometry");
		
		for (int y=0; y<height; y++)
		{
			for (int x=0; x<width; x++)
			{			
				//float p1 = heightfield[y*(width+2)+x];
				//float p2 = heightfield[y*(width+2)+x+1];
				//float p3 = heightfield[(y+1)*(width+2)+x];
				//float p4 = heightfield[(y+1)*(width+2)+x+1];
	
				Vector3 vertex;
				vertex.x = mapsize*x/(width-2.0f) - mapsize * 0.5f;
				vertex.z = mapsize*y/(height-2.0f) - mapsize * 0.5f;
				vertex.y = heightfield[y*(width+2)+x];
				
				vertices[y*width + x] = vertex;
				Vector2 temp_uv = new Vector2((float)x, (float)y);
				uv[y*width + x] = Vector2.Scale(temp_uv, uvScale);
			}
		}
		
		Color col = Color.white;
		for (int y=0; y<texSize; y++)
		{
			for (int x=0; x<texSize; x++)
			{
				float rand_unit = Random.value;
				rand_unit *= 0.1f;
				rand_unit += 0.9f;
				col.r = col.g = col.b = rand_unit;
				texture.SetPixel(x, y, col);
			}
		}
		
		// Pass 4 - build index buffer
		Debug.Log(" Pass 4 - build index buffer");
		
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
					triangles[index++] = (y     * width) + x;
					triangles[index++] = ((y+1) * width) + x;
					triangles[index++] = (y     * width) + x + 1;
			
					triangles[index++] = ((y+1) * width) + x;
					triangles[index++] = ((y+1) * width) + x + 1;
					triangles[index++] = (y     * width) + x + 1;
				}
			}
		}
		// And assign them to the mesh
		mesh.triangles = triangles;
			
		// Auto-calculate vertex normals from the mesh
		mesh.RecalculateNormals();
		
		MeshCollider collider = transform.GetComponent<MeshCollider>();
		collider.sharedMesh = emptyMesh;	// Flush cached physics-friendly version of old mesh
		collider.sharedMesh = mesh;
		
		// Pass 5 - populate surface features
		Debug.Log(" Pass 5 - populate surface features");
		
		for (int i=0; i<treeCount; i++)
		{
			Vector2 pos = Random.insideUnitCircle;
			pos *= mapsize * 0.4f;
			
			float y = GetTerrainHeight( pos.x, pos.y );
			Instantiate( treePrefab, new Vector3(pos.x, y, pos.y), Quaternion.identity );
		}
		
		//
		// Place the MegaTree on the highest point in the map
		//
		Vector3 megaPos;
		megaPos.x = mapsize*highestx/(width-2.0f) - mapsize * 0.5f;
		megaPos.z = mapsize*highesty/(height-2.0f) - mapsize * 0.5f;
		megaPos.y = heightfield[highesty*(width+2)+highestx];
		
		megaTree = Instantiate( treePrefab, megaPos, Quaternion.identity ) as GameObject;
		megaTree.GetComponent<TreeLogic>().SetMegaTree( treeCount );
		
		
		Debug.Log("Finished mesh generation.");
	}
	
	public GameObject GetMegaTree()
	{
		return megaTree;
	}
	
	public bool IsCompleted()
	{
		if (megaTree.GetComponent<TreeLogic>().IsCompleted())
			return true;
	
		return false;
	}
	
	public void ColourTexture( Vector3 pos, float radius, Color col )
	{
		float px = texSize*uvFudge*(pos.x + mapsize * 0.5f)/mapsize;
		float py = texSize*uvFudge*(pos.z + mapsize * 0.5f)/mapsize;
		
		if (radius == 0.0f)
		{
			Color tweakcol = texture.GetPixel( (int)px, (int)py );
			tweakcol = Color.Lerp( tweakcol, col, col.a*30.0f*Time.deltaTime );
			texture.SetPixel( (int)px, (int)py, tweakcol );
			return;
		}
		
		float pr = texSize*uvFudge*(radius)/mapsize;
		Color tcol = col;
		
		for (float i=px-pr; i<px+pr; i+=1.0f)
		{
			for (float j=py-pr; j<py+pr; j+=1.0f)
			{
				float r2 = (i-px)*(i-px)+(j-py)*(j-py);
				if (r2 < pr*pr && (pr <= 4.0f || r2 > (pr-1.5f)*(pr-1.5f)))
				{
					float rand_unit = Random.value;
					rand_unit *= 0.1f;
					rand_unit += 0.9f;
					tcol.r = col.r * rand_unit;
					tcol.g = col.g * rand_unit;
					tcol.b = col.b * rand_unit;
					
					Color tweakcol = texture.GetPixel( (int)i, (int)j );
					tcol = Color.Lerp( tweakcol, tcol, col.a*30.0f*Time.deltaTime );
					
					texture.SetPixel( (int)i, (int)j, tcol );
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if ((framecount & 0x00000001) > 0)
			texture.Apply( false );	// In case anything changed the texture
		framecount++;
	}
}
