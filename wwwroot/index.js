var camera;
var controls;
var group;

window.JSapi = {
    initializeScene: function (canvasId) {
        const canvas = document.getElementById(canvasId);

        // シーン、カメラ、レンダラーの設定
        const scene = new THREE.Scene();
        camera = new THREE.PerspectiveCamera(75, canvas.clientWidth / canvas.clientHeight, 0.1, 1000);
        const renderer = new THREE.WebGLRenderer({ canvas: canvas });
        renderer.setSize(canvas.clientWidth, canvas.clientHeight);
        renderer.setPixelRatio(window.devicePixelRatio);


        { // 照明
            const light = new THREE.DirectionalLight(0xffffff, 1);
            light.position.set(1, 1, 1).normalize();
            scene.add(light);
        }
        { // 照明
            const light = new THREE.HemisphereLight(0x888888, 0x101050, 1.0);
            scene.add(light);
        }
        { // グリッド
            const size = 100;
            const divisions = 20;

            const gridHelper = new THREE.GridHelper( size, divisions );
            scene.add( gridHelper );
        }
        {
            const axisHelper = new THREE.AxisHelper(10);
            scene.add(axisHelper);
        }

        // PointerLockControlsを使ってカメラをコントロール
        controls = new THREE.PointerLockControls(camera, document.body);

        TreeGroup = new THREE.Group();
        scene.add(TreeGroup);

        // クリックしてポインタロックを開始
        document.addEventListener('click', () => {
            controls.lock();
        });
        var cylinder;
        {
            const geometry = new THREE.CylinderGeometry(1, 1, 10, 32);
            const material = new THREE.MeshStandardMaterial({ color: 0xa04010 });
            cylinder = new THREE.Mesh(geometry, material);

            // シーンに円柱を追加
            TreeGroup.add(cylinder);
            cylinder.position.z = 1;
            cylinder.position.y = 2;
            cylinder.rotation.x = 1;
        }

        // カメラの初期位置を設定
        camera.position.z = 6;
        camera.position.y = 2;

        // ウィンドウサイズに応じたリサイズ処理
        window.addEventListener('resize', onresize);
        function onresize() {
            const width = window.innerWidth-20;
            const height = window.innerHeight-100;
            renderer.setSize(width, height);
            camera.aspect = width / height;
            camera.updateProjectionMatrix();
        }
        // アニメーションループ
        function animate() {
            requestAnimationFrame(animate);
            TreeGroup.rotation.y += 0.01;
            renderer.render(scene, camera);
        }

        onresize();
        animate();
        draw()
    },
    log: function (...arg) {
        console.log(...arg);
    }
};


var prevTime = performance.now();
const moveSpeed = 10;
const keys = {};
const keymap = {
    "forward": ["e","w"],
    "back": ["i","s"],
    "left": ["u","a"],
    "right": ["f","d"],
    "up": [","],
    "down": ["."],
}

function draw() { // アニメーションループ
    requestAnimationFrame(draw);
    //controls.update(clock.getDelta());

    const time = performance.now();
    info_fps.innerText = 1000/(time-prevTime)
    if (controls.isLocked) {
        const delta = (time - prevTime) / 1000;

        const moveDistance = moveSpeed * delta;

        // カメラの向きベクトルを取得
        const cameraDirection = new THREE.Vector3();
        camera.getWorldDirection(cameraDirection).projectOnPlane(camera.up).normalize();

        // WASDで前進/後退
        if (lookupkeydown(keymap.forward)) camera.position.addScaledVector(cameraDirection, moveDistance);
        if (lookupkeydown(keymap.back)) camera.position.addScaledVector(cameraDirection, -moveDistance);

        // 左右移動
        const cameraRight = new THREE.Vector3();
        camera.getWorldDirection(cameraRight).cross(camera.up);
        if (lookupkeydown(keymap.left)) camera.position.addScaledVector(cameraRight, -moveDistance);
        if (lookupkeydown(keymap.right)) camera.position.addScaledVector(cameraRight, moveDistance);
        if (lookupkeydown(keymap.up)) camera.position.y += moveDistance;
        if (lookupkeydown(keymap.down)) camera.position.y -= moveDistance;
    }
    prevTime = performance.now();
    //console.log(camera)
}
function lookupkeydown(keyarr) {
    for (let i of keyarr) {
        if (keys[i]) {
            return true
        }
    }
    return false
}


document.addEventListener('keydown', (e) => {
    keys[e.key.toLowerCase()] = true;
});

document.addEventListener('keyup', (e) => {
    keys[e.key.toLowerCase()] = false;
});