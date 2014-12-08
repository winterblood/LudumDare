using UnityEngine;
using System.Collections;

public class PolyMap : MonoBehaviour
{
	public int mapSizeX = 20;
	public int mapSizeY = 20;
	public Material mtl;
	public float lensRadius = 6.0f;
	public int lensMagnify = 4;
	public float lensStrength = 0.5f;
	
	private Vector3 lensFocus = Vector3.zero;
	
	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] uvs;
	private int[] triangles;
	
	private Mesh mesh;
	private Vector2 topleft 	= new Vector2( 0.0f, 0.0f );
	private Vector2 topright	= new Vector2( 1.0f, 0.0f );
	private Vector2 botright	= new Vector2( 1.0f, 1.0f );
	private Vector2 botleft		= new Vector2( 0.0f, 1.0f );
	
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
		normals[vc]		= -Vector3.forward;
		vertices[vc++] 	= v1;
		uvs[vc] 		= topright;
		normals[vc]		= -Vector3.forward;
		vertices[vc++] 	= v2;
		uvs[vc] 		= botright;
		normals[vc]		= -Vector3.forward;
		vertices[vc++] 	= v3;
		uvs[vc] 		= botleft;
		normals[vc]		= -Vector3.forward;
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
				float z = 0.0f; //Random.Range(0.0f, 0.4f);
				v1 = new Vector3( (i)*1.0f,   (j)*1.0f, z );
				v2 = new Vector3( (i+1)*1.0f, (j)*1.0f, z );
				v3 = new Vector3( (i+1)*1.0f, (j+1)*1.0f, z );
				v4 = new Vector3( (i)*1.0f,   (j+1)*1.0f, z );
				AddQuad(ref v1,
				        ref v2,
				        ref v3,
				        ref v4,
						ref tricount,
						ref vertcount );
			}
		}
		Vector3 v = new Vector3( (float)mapSizeX * -0.5f, (float)mapSizeY * -0.5f, 0.0f );
		transform.position = v;
		lensFocus = -v;
		
		mesh.vertices 	= vertices;
		mesh.uv 		= uvs;
		mesh.triangles 	= triangles;
		mesh.normals	= normals;
		mesh.RecalculateBounds();
	}

	void DistortPoint( ref int vertindex, ref Vector3 v1 )
	{
		Vector3 d = v1 - lensFocus;
		d.z = 0.0f;
	
		if (d.sqrMagnitude < lensRadius*lensRadius)
		{
			float distance = d.magnitude;
			float ndistance = distance / lensRadius;
			float ndistToEdge = 1.0f - ndistance;
			float skew = ndistToEdge * lensStrength;
						
			float delta = ndistToEdge * lensRadius * skew;			
			d.Normalize();
			d *= delta;
			v1 += d;
		}
		vertices[vertindex++] 	= v1;		
	}

	void RegenVerts()
	{
		int vertindex = 0;
		Vector3 v1, v2, v3, v4;
		for (int i=0; i<mapSizeX; i++)
		{
			for (int j=0; j<mapSizeY; j++)
			{
				float z = 0.0f; //Random.Range(0.0f, 0.4f);
				v1 = new Vector3( (i)*1.0f,   (j)*1.0f, z );
				v2 = new Vector3( (i+1)*1.0f, (j)*1.0f, z );
				v3 = new Vector3( (i+1)*1.0f, (j+1)*1.0f, z );
				v4 = new Vector3( (i)*1.0f,   (j+1)*1.0f, z );
				
				DistortPoint( ref vertindex, ref v1 );
				DistortPoint( ref vertindex, ref v2 );
				DistortPoint( ref vertindex, ref v3 );
				DistortPoint( ref vertindex, ref v4 );
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