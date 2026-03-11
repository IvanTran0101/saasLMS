mkcert "saaslms.dev" "*.saaslms.dev" 
kubectl create namespace saaslms
kubectl create secret tls -n saaslms saaslms-tls --cert=./saaslms.dev+1.pem  --key=./saaslms.dev+1-key.pem