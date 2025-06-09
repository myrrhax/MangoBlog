import { Output } from "@editorjs/react-renderer";

export default function PostView(data) {
  return (
    <Output
      data={data}
      config={{
        video: {
          render: (block) => (
            <div className="my-4">
              <iframe
                width="560"
                height="315"
                src={block.data.url}
                frameBorder="0"
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                allowFullScreen
              />
            </div>
          ),
        },
      }}
    />
  );
}