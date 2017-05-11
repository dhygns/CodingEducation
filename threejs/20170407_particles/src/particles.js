
class Particles extends THREE.Object3D{

  constructor(position, velocity, life, cnt) {
    super();

    this._position = position;
    this._velocity = velocity;
    this._life = life;

    this.unif = {
      unif_posx : { type : "t", value : this._position.x },
      unif_posy : { type : "t", value : this._position.y },
      unif_posz : { type : "t", value : this._position.z },
    };

    this.geom = new THREE.InstancedBufferGeometry();
    this.indices = [];

    //This Plane Mesh
    const buffer_count_ = 6;
    const id_width_ = Math.min(cnt, 16384);
    const id_height_ = Math.min(Math.floor(cnt / 16384) + 1, 16384);

    for(var idx = 0; idx < cnt; idx ++) {
      this.indices.push(idx);
    }

    this.verta = new THREE.BufferAttribute(new Float32Array(Cube_Vertices), 3);
    this.norma = new THREE.BufferAttribute(new Float32Array(Cube_Normals), 3);
    this.idxa = new THREE.InstancedBufferAttribute(new Float32Array(this.indices), 1, 1);
    this.geom.addAttribute('position', this.verta);
    this.geom.addAttribute('normal', this.norma);
    this.geom.addAttribute('indices', this.idxa);
    this.matr = new THREE.ShaderMaterial({
      transparent : true,
      side : THREE.DoubleSide,
      // depthWrite : false,
      uniforms : this.unif,
      vertexShader : m_float + `
      attribute float indices;

      uniform sampler2D unif_posx;
      uniform sampler2D unif_posy;
      uniform sampler2D unif_posz;

      ` + Vec4ToFloat_for_GLSL + `
      vec2 idx() {
        float id = indices;
        float idw = ` + id_width_ + `.0;
        float idh = ` + id_height_ + `.0;
        return vec2(
          (floor(mod(id, 16384.0)) + 0.5) / idw,
          (floor(id / 16384.0) + 0.5) / idh);
      }

      float getFloat(sampler2D s) {
        return Vec4ToFloat(texture2D(s, idx()));
      }

      // mat4
      mat4 scale(float s) {
        mat4 m;
        m[0] = vec4(  s, 0.0, 0.0, 0.0);
        m[1] = vec4(0.0,   s, 0.0, 0.0);
        m[2] = vec4(0.0, 0.0,   s, 0.0);
        m[3] = vec4(0.0, 0.0, 0.0, 1.0);
        return m;
      }

      mat4 rotateX(float x) {
        mat4 m;
        m[0] = vec4(1.0, 0.0, 0.0, 0.0);
        m[1] = vec4(0.0, cos(x), -sin(x), 0.0);
        m[2] = vec4(0.0, sin(x),  cos(x), 0.0);
        m[3] = vec4(0.0, 0.0, 0.0, 1.0);
        return m;
      }

      mat4 trans() {
        float x = (getFloat(unif_posx) - M_FLOAT) * 2.0 * 255.0;
        float y = (getFloat(unif_posy) - M_FLOAT) * 2.0 * 255.0;
        float z = (getFloat(unif_posz) - M_FLOAT) * 2.0 * 255.0;

        mat4 m;
        m[0] = vec4(1.0, 0.0, 0.0, 0.0);
        m[1] = vec4(0.0, 1.0, 0.0, 0.0);
        m[2] = vec4(0.0, 0.0, 1.0, 0.0);
        m[3] = vec4(  x,   y,   z, 1.0);
        return m;
      }

      varying vec2 vtex;
      varying vec3 vnor;

      void main(void) {
        vec4 vertex = vec4(position, 1.0);
        mat4 particleMatrix = trans() * rotateX(idx().x) * scale(0.4);

        vtex = vertex.xy * 0.5 + 0.5;
        vnor = normal;
        gl_Position = projectionMatrix * viewMatrix * particleMatrix * vertex;
      }
      `,
      fragmentShader : `
      varying vec2 vtex;
      varying vec3 vnor;
      void main(void){

        vec3 light = normalize(vec3(1.0, 2.0, 3.0));
        vec3 bright = vec3(dot(light, vnor));

        gl_FragColor = vec4(bright, 1.0);
      }
      `
    });

    this.add(new THREE.Mesh(this.geom, this.matr));
    // console.log(this.geom);

  }

}
