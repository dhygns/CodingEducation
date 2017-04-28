
class Brush extends THREE.Object3D{
  constructor(memory, scene) {
    super();

    this.memory = memory;
    this.scene = scene;
    this.depth = 1;
    this.velocity = { x : 0.0, y : 0.0 };
    this.lifecycle = 1.0 + Math.random() * 1.0;
    this.scale.x = this.scale.y = 0.04;
    this.uniforms = {

    };
    this.add(new THREE.Mesh(
      new THREE.PlaneGeometry(1.0, 1.0),
      new THREE.ShaderMaterial({
        uniforms : { },
        transparent : true,
        vertexShader : `
        varying vec2 vary_tex;
        void main(void) {
          vary_tex = uv;
          gl_Position = projectionMatrix * modelViewMatrix * vec4(position.xy, 0.0, 1.0);
        }
        `,
        fragmentShader : `
        varying vec2 vary_tex;

        float rand(vec2 co){
          return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
        }

        float brush(vec2 st) {


          return
            smoothstep(0.5 - 0.01, 0.5 + 0.01, st.y);
        }



        void main(void) {
          vec2 st = vary_tex * 2.0 - 1.0;
          float alpha = brush(st);
          gl_FragColor = vec4(0.0, 0.0, 0.0, alpha * 0.1);
        }
        `
      })
    ));
  }

  init(depth) {

    if(depth == undefined) {
      depth = 1;

      this.position.x = 0.0;
      this.position.y = -4.0;
    }

    this.depth = depth;
    this.lifecycle = 1.0 + Math.random() * 1.0;
    return this;
  }

  update(dt) {
    this.lifecycle -= dt;

    this.position.x += this.velocity.x * dt;
    this.position.y += this.velocity.y * dt;

    this.rotation.z = Math.random() * 2.0 * Math.PI;
    this.scale.x = this.scale.y = 0.04 + 0.1 * Math.random();


    if(this.lifecycle < 0.0) {
      if(this.depth > 11) this.remove();
      else this.seperate();
    }
  }


  seperate() {
    const other = this.memory.pop().init(this.depth + 1);
    this.scene.add(other);
    this.depth = this.depth + 1;

    other.position.x = this.position.x;
    other.position.y = this.position.y;

    this.velocity.x = Math.random() - 0.5;
    this.velocity.y = Math.random() * 0.5;

    other.velocity.x = Math.random() - 0.5;
    other.velocity.y = Math.random() * 0.5;
  }

  remove() {
    this.scene.remove(this);
    this.memory.push(this);
  }


}
