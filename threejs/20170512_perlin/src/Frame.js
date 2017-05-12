

class Frame {
  constructor(rdrr) {
    this.rdrr = rdrr;
    this.perlin = new Perlin(16, 16);

    this.camera = new THREE.Camera();
    this.scene = new THREE.Scene();

    this.mesh = new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.ShaderMaterial({
        uniforms : { unif_texture : { type : "t", value : this.perlin.Texture }},
        vertexShader : `
        varying vec2 vtex;
        void main(void) {
          vtex = uv;
          gl_Position = vec4(position, 1.0);
        }
        `,
        fragmentShader : `
        uniform sampler2D unif_texture;
        varying vec2 vtex;
        void main(void) {
          float brightness = texture2D(unif_texture, vtex).a;
          gl_FragColor = vec4(vec3(brightness), 1.0);
        }
        `
      })
    );

    this.scene.add(this.mesh);
  }

  update(dt) {
    this.perlin.update();
    // console.log(dt);
  }

  render() {
    this.rdrr.render(this.scene, this.camera);
  }
}
