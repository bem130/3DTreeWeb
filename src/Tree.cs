using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

// 座標系はThree.jsのものを使用

namespace TreeModel {
    public abstract class TreeNode
    {
        public TreeNode? Parent { get; set; } // 親ノード
        public List<TreeNode> Children { get; set; } // 子ノード
        public double length { get; set; }
        public Vector3 angle { get; set; }
        public double thickness { get; set; }

        protected TreeNode()
        {
            Children = new List<TreeNode>();
        }

        /// <summary>
        /// 木に新たな枝を追加ぃます
        /// </summary>
        public void AddChild(TreeNode child)
        {
            child.Parent = this; // 親を設定
            Children.Add(child);
        }

        public abstract Vector3 getPos();
        public abstract Vector3 getAng();
        public abstract string Display(int depth = 0);
        public abstract List<Vector3> DisplayPos();
    }

    public class BranchNode: TreeNode
    {

        /// <summary>
        /// 木の枝
        /// </summary>
        /// <param name="angle">親ノードからの相対的な角度を指定します</param>
        /// <param name="length">親ノードからの相対的な長さを指定します</param>
        /// <param name="thickness">親ノードからの相対的な太さを指定します</param>
        public BranchNode(Vector3 angle,double length,double thickness)
        {
            this.length = length;
            this.angle = angle;
            this.thickness = thickness;
        }
        public override Vector3 getPos()
        {
            return Parent.getPos() + getAng() * (float)length;
        }
        public override Vector3 getAng()
        {
            return Vector3.Normalize(Vector3.Transform( Parent.getAng(), EulerToRotationMatrix(angle)));
        }
        public override string Display(int depth = 0)
        {
            string res = "";
            res += new String(' ', depth*4) + "[BranchNode]\n";
            res += new String(' ', depth*4) + "| Angle     : " + angle +"\n";
            res += new String(' ', depth*4) + "| Length    : " + length +"\n";
            res += new String(' ', depth*4) + "| Thickness : " + thickness +"\n";
            res += new String(' ', depth*4) + "|+ Global Pos   : " + getPos() +"\n";
            res += new String(' ', depth*4) + "|+ Global Angle : " + getAng() +"\n";
            res += "\n";
            foreach (var child in Children)
            {
                res += child.Display(depth + 1);
            }
            return res;
        }
        public override List<Vector3> DisplayPos()
        {
            var list = new List<Vector3>();
            foreach (var child in Children)
            {
                list.AddRange(child.DisplayPos());
            }
            list.Add(getPos());
            return list;
        }
        static Matrix4x4 EulerToRotationMatrix(Vector3 euler)
        {
            float radX = euler.X;
            float radY = euler.Y;
            float radZ = euler.Z;
            Matrix4x4 rotX = new Matrix4x4(
                1, 0, 0, 0,
                0, (float)Math.Cos(radX), -(float)Math.Sin(radX), 0,
                0, (float)Math.Sin(radX), (float)Math.Cos(radX), 0,
                0, 0, 0, 1
            );
            Matrix4x4 rotY = new Matrix4x4(
                (float)Math.Cos(radY), 0, (float)Math.Sin(radY), 0,
                0, 1, 0, 0,
                -(float)Math.Sin(radY), 0, (float)Math.Cos(radY), 0,
                0, 0, 0, 1
            );
            Matrix4x4 rotZ = new Matrix4x4(
                (float)Math.Cos(radZ), -(float)Math.Sin(radZ), 0, 0,
                (float)Math.Sin(radZ), (float)Math.Cos(radZ), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
            return Matrix4x4.Multiply(rotZ, Matrix4x4.Multiply(rotY, rotX));
        }

    }

    public class Tree: TreeNode
    {

        /// <summary>
        /// 木
        /// </summary>
        /// <param name="angle">絶対的な角度を指定します</param>
        /// <param name="length">絶対的な長さを指定します</param>
        /// <param name="thickness">絶対的な太さを指定します</param>
        public Tree(Vector3 angle,double length,double thickness)
        {
            this.length = length;
            this.angle = angle;
            this.thickness = thickness;
        }

        public override Vector3 getPos()
        {
            return new Vector3(0,0,0);
        }
        public override Vector3 getAng()
        {
            return Vector3.Normalize(angle);
        }

        public override string Display(int depth = 0)
        {
            string res = new String(' ', depth*4) + "[RootNode]\n";
            res += new String(' ', depth*4) + "| Angle     : " + angle +"\n";
            res += new String(' ', depth*4) + "| Length    : " + length +"\n";
            res += new String(' ', depth*4) + "| Thickness : " + thickness +"\n\n";
            foreach (var child in Children)
            {
                res += child.Display(depth + 1);
            }
            return res;
        }
        public override List<Vector3> DisplayPos()
        {
            var list = new List<Vector3>();
            foreach (var child in Children)
            {
                list.AddRange(child.DisplayPos());
            }
            list.Add(getPos());
            return list;
        }

    }
}