using UnityEngine;
using System.Collections;

public class PolySea : MonoBehaviour
{
	public int mapSizeX = 60;
	public int mapSizeY = 50;
	public Material mtl;
	
	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] uvs;
	private int[] triangles;
	
	private Mesh mesh;
	private Vector2 topleft 	= new Vector2( 0.0f, 0.0f );
	private Vector2 topright	= new Vector2( 1.0f, 0.0f );
	private Vector2 botright	= new Vector2( 1.0f, 1.0f );
	private Vector2 botleft		= new Vector2( 0.0f, 1.0f );
	
	private float time = 0.0f;
	
	void AddQuad( ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4, ref int index, ref int vc)
	{
		// Top
		triangles[index++] = vc;
		triangles[index++] = vc+2;
		triangles[index++] = vc+1;
		
		triangles[index++] = vc;
		triangles[index++] = vc+3;			
		triangles[index++] = vc+2;
		
		uvs[vc] 		= topleft;
		normals[vc]		= Vector3.up;
		vertices[vc++] 	= v1;
		uvs[vc] 		= topright;
		normals[vc]		= Vector3.up;
		vertices[vc++] 	= v2;
		uvs[vc] 		= botright;
		normals[vc]		= Vector3.up;
		vertices[vc++] 	= v3;
		uvs[vc] 		= botleft;
		normals[vc]		= Vector3.up;
		vertices[vc++] 	= v4;
	}
	
	void Start()
	{
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		
		vertices 	= new Vector3[mapSizeX * mapSizeY * 4];
		uvs 		= new Vector2[mapSizeX * mapSizeY * 4];
		normals 	= new Vector3[mapSizeX * mapSizeY * 4];
		triangles 	= new int[mapSizeX * mapSizeY * 6];	// Indices		
		
		mesh = new Mesh();
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh = mesh;
		mf.renderer.material = mtl;		
		
		int vertcount = 0;
		int tricount = 0;
		Vector3 v1, v2, v3, v4;
		for (int i=0; i<mapSizeX; i++)
		{
			for (int j=0; j<mapSizeY; j++)
			{
				v1 = new Vector3( (i)*1.0f,   0.0f, (j)*1.0f );
				v2 = new Vector3( (i+1)*1.0f, 0.0f, (j)*1.0f );
				v3 = new Vector3( (i+1)*1.0f, 0.0f, (j+1)*1.0f );
				v4 = new Vector3( (i)*1.0f,   0.0f, (j+1)*1.0f );
				AddQuad(ref v1,
				        ref v2,
				        ref v3,
				        ref v4,
				        ref tricount,
				        ref vertcount );
			}
		}
		Vector3 v = new Vector3( (float)mapSizeX * -0.5f, -0.2f, (float)mapSizeY * -0.5f );
		transform.position = v;
		
		mesh.vertices 	= vertices;
		mesh.uv 		= uvs;
		mesh.triangles 	= triangles;
		mesh.normals	= normals;
		mesh.RecalculateBounds();
	}
	
	void RegenVerts()
	{
		int vertindex = 0;
		time += Time.deltaTime;
		float t=time * Mathf.PI;
		Vector3 v0, v1, v2, v3;
		for (int i=0; i<mapSizeX; i++)
		{
			for (int j=0; j<mapSizeY; j++)
			{
				v0 = new Vector3( (i)*1.0f,   Mathf.Sin( ((float)(i-j)+t) * Mathf.PI * 0.15f ) * Mathf.Sin( ((float)(i+j)+t) * Mathf.PI * 0.4f ) * 0.15f, (j)*1.0f );
				v1 = new Vector3( (i+1)*1.0f, Mathf.Sin( ((float)(i+1-j)+t) * Mathf.PI * 0.15f ) * Mathf.Sin( ((float)(i+1+j)+t) * Mathf.PI * 0.4f ) * 0.15f, (j)*1.0f );
				v2 = new Vector3( (i+1)*1.0f, Mathf.Sin( ((float)(i+1-j-1)+t) * Mathf.PI * 0.15f ) * Mathf.Sin( ((float)(i+1+j+1)+t) * Mathf.PI * 0.4f ) * 0.15f, (j+1)*1.0f );
				v3 = new Vector3( (i)*1.0f,   Mathf.Sin( ((float)(i-j-1)+t) * Mathf.PI * 0.15f ) * Mathf.Sin( ((float)(i+j+1)+t) * Mathf.PI * 0.4f ) * 0.15f, (j+1)*1.0f );
				
				Vector3 n1 = Vector3.Cross ( v2-v0, v1-v0 );
				Vector3 n2 = Vector3.Cross ( v3-v0, v2-v0 );
				
				normals[vertindex]		= n1;
				vertices[vertindex++] 	= v0;
				normals[vertindex]		= (n1+n2) * 0.5f;
				vertices[vertindex++] 	= v1;
				normals[vertindex]		= (n1+n2) * 0.5f;
				vertices[vertindex++] 	= v2;
				normals[vertindex]		= n2;
				vertices[vertindex++] 	= v3;
			}
		}
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh.vertices = vertices;
	}
	
	void Update ()
	{
		RegenVerts();
	}
}